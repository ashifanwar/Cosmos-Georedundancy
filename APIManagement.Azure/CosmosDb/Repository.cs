using APIManagement.Contract;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace APIManagement.Azure.CosmosDb
{
    public class Repository<T>  where T : APIMBase
    {
        public readonly DocumentClient _writeClient;
        public readonly DocumentClient _readClient;
        public readonly DocumentClient _secondaryReadClient;

        protected readonly DocumentCollection _writeCollection;
        protected readonly DocumentCollection _readCollection;
        protected readonly DocumentCollection _secondatyReadCollection;

        protected readonly Database _writeDatabase;
        protected readonly Database _readDatabase;
        protected readonly Database _secondaryReadDatabase;

        public Repository(CosmosDbSettings primarySettings, CosmosDbSettings secondarySettings)
        {
            var writeClientPolicy = new ConnectionPolicy
            {
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            };
            writeClientPolicy.PreferredLocations.Add(primarySettings.PrefferedLocation);
            writeClientPolicy.PreferredLocations.Add(secondarySettings.PrefferedLocation);

            _writeClient = new DocumentClient(new Uri(primarySettings.EndpointUrl), primarySettings.AuthKey, writeClientPolicy);
            _writeDatabase = GetDatabase(_writeClient, primarySettings.DatabaseName);
            _writeCollection = GetCollection(_writeClient, _writeDatabase.SelfLink, primarySettings.CollectionName);

            var readClientPolicy = new ConnectionPolicy
            {
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            };
            readClientPolicy.PreferredLocations.Add(secondarySettings.PrefferedLocation);
            readClientPolicy.PreferredLocations.Add(primarySettings.PrefferedLocation);
            _readClient = new DocumentClient(new Uri(secondarySettings.EndpointUrl), secondarySettings.AuthKey, readClientPolicy);
            _readDatabase = GetDatabase(_readClient, secondarySettings.DatabaseName);
            _readCollection = GetCollection(_readClient, _readDatabase.SelfLink, secondarySettings.CollectionName);

            var secondaryRegionPolicy = new ConnectionPolicy
            {
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            };
            secondaryRegionPolicy.PreferredLocations.Add(secondarySettings.PrefferedLocation);
            _secondaryReadClient = new DocumentClient(new Uri(secondarySettings.EndpointUrl), secondarySettings.AuthKey, secondaryRegionPolicy);
            _secondaryReadDatabase = GetDatabase(_secondaryReadClient, secondarySettings.DatabaseName);
            _secondatyReadCollection = GetCollection(_secondaryReadClient, _secondaryReadDatabase.SelfLink, secondarySettings.CollectionName);

        }

        public async Task<T> CreateAsync(T entity)
        {
            var response = await _writeClient.CreateDocumentAsync(_writeCollection.SelfLink, entity);
            Document document = response;
            var created = (T)(dynamic)document;
            return created;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var query = GetQueryableItemsAsync().AsDocumentQuery();
            var result = new List<T>();

            while(query.HasMoreResults)
            {
                var response = await query.ExecuteNextAsync<T>();
                result.AddRange(response);
            }

            return result;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var favoredDoc = default(T);
            var secondaryDoc = default(T);
            var taskArray = new Task[2];
            taskArray[0] = Task.Run(() =>
            {
                try
                {
                    var response = _writeClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(_writeDatabase.Id, _writeCollection.Id, id)).Result;

                    Document document = response;
                    favoredDoc = (T)(dynamic)document;
                }
                catch(Exception exception)
                {
                    if (exception is DocumentClientException &&
                        ((DocumentClientException)exception).StatusCode == HttpStatusCode.NotFound ||
                        (exception is AggregateException && exception.InnerException.Message.Contains("Found")))
                    {
                        favoredDoc = default(T);
                    }
                    else
                    {
                        throw;
                    }
                }
            });

            taskArray[1] = Task.Run(() =>
            {
                try
                {
                    var response = _readClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(_readDatabase.Id, _readCollection.Id, id)).Result;
                    Document document = response;
                    secondaryDoc = (T)(dynamic)document;
                }
                catch(Exception exception)
                {
                    if (exception is DocumentClientException && ((DocumentClientException)exception).StatusCode == HttpStatusCode.NotFound ||
                        (exception is AggregateException && exception.InnerException.Message.Contains("Found")))
                    {
                        secondaryDoc = default(T);
                    }
                    else
                    {
                        throw;
                    }
                }
            });
            var doc = default(T);
            Task.WaitAll(taskArray);
            if(favoredDoc == null && secondaryDoc == null)
            {
                try
                {
                    var response = await _secondaryReadClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(_secondaryReadDatabase.Id, _secondatyReadCollection.Id, id));
                    Document document = response;
                    var result = (T)(dynamic)document;
                    return result;
                }
                catch(Exception ex)
                {
                    return default(T); 
                }
            }
            if(favoredDoc != null && secondaryDoc == null)
            {
                return favoredDoc;
            }
            if(favoredDoc == null)
            {
                return secondaryDoc;
            }
            if(favoredDoc != null && secondaryDoc != null)
            {
               doc = favoredDoc.SequenceNumber > secondaryDoc.SequenceNumber ? favoredDoc : secondaryDoc;
            }
            return doc;                        
        }


        //public async Task<T> Update(string id, T entity)
        //{
        //    var response = await Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(Database.Id, Collection.Id, id), entity, new RequestOptions { PartitionKey = new PartitionKey(id) });
        //    Document document = response;
        //    T updated = (T)(dynamic)document;
        //    return updated;            
        //}

        private IQueryable<T> GetQueryableItemsAsync()
        {
            return _writeClient.CreateDocumentQuery<T>(_writeCollection.DocumentsLink);
        }




        //private DocumentClient GetDocumentClient()
        //{
            
        //      var connectionPolicy = new ConnectionPolicy
        //      {
        //          ConnectionProtocol = Protocol.Tcp,
        //          EnableEndpointDiscovery = true,
        //          ConnectionMode = ConnectionMode.Direct
        //      };

            
        //      connectionPolicy.PreferredLocations.Add(CosmosDbSettings.PrefferedLocation);
            

        //    var client = new DocumentClient(new Uri(CosmosDbSettings.EndpointUrl), CosmosDbSettings.AuthKey, connectionPolicy);

        //    return client;
        //}

        protected Database GetDatabase(DocumentClient client, string databaseId)
        {           
            Database database = client.CreateDatabaseQuery().Where(x => x.Id == databaseId).ToArray().FirstOrDefault();
            if (database == null)
            {
                throw new ApplicationException("Database does not exist");
            }            
            return database;
        }

        protected DocumentCollection GetCollection(DocumentClient client, string databaseLink, string collectionId)
        {
            DocumentCollection collection = client.CreateDocumentCollectionQuery(databaseLink).Where(x => x.Id == collectionId).ToArray().FirstOrDefault();
            if (collection == null)
            {
                throw new ApplicationException("Collection does not exist");
            }
            return collection;
        }

        protected FeedOptions DefaultFeedOptions
        {
            get
            {
                return new FeedOptions
                {
                    EnableCrossPartitionQuery = true,
                    PopulateQueryMetrics = true
                };
            }
        }
    }
}
