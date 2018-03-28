using APIManagement.Contract;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIManagement.Azure.CosmosDb
{
    public class PersonalInfoRespository :Repository<PersonalInfo>, IRepository<PersonalInfo>
    {
        protected readonly Repository<PersonalInfo> eastRepo;
        protected readonly Repository<PersonalInfo> westRepo;
        protected readonly Repository<PersonalInfo> Repo;

        public PersonalInfoRespository(CosmosDbPrimaryAccountSettings primarySettings, CosmosDbSecondaryAccountSettings secondarySettings) : base(primarySettings, secondarySettings)
        {
            eastRepo = new Repository<PersonalInfo>(primarySettings, secondarySettings);
            westRepo = new Repository<PersonalInfo>(secondarySettings, primarySettings);
        }

        public async Task<PersonalInfo> CreateItemOnEastAsync(PersonalInfo item)
        {
            return await eastRepo.CreateAsync(item);
        }

        public async Task<PersonalInfo> CreateItemOnWestAsync(PersonalInfo item)
        {
            return await westRepo.CreateAsync(item);
        }

        public async Task<PersonalInfo> CreateItemAsync(PersonalInfo item)
        {
            return await CreateAsync(item);
        }

        public async Task<IEnumerable<PersonalInfo>> GetAllItem()
        {
            return await eastRepo.GetAllAsync();
        }

        public async Task<PersonalInfo> GetItemByIdEastAsync(string id)
        {
            PersonalInfo item = null;
            item = await eastRepo.GetByIdAsync(id);
            
            return item;
        }

        public async Task<PersonalInfo> GetItemByIdWestAsync(string id)
        {
            PersonalInfo item = null;
            item = await westRepo.GetByIdAsync(id);
            
            return item;
        }

        public async Task<PersonalInfo> GetItemByIdAsync(string id)
        {
            PersonalInfo item = null;
            item = await GetByIdAsync(id);
            
            return item;
        }

        public Task<PersonalInfo> Update(string id, PersonalInfo item)
        {
            throw new NotImplementedException();
        }
    }
}
