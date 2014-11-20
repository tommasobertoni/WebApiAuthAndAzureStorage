using Bertoni.TestFinale.Activity.AzureStorage.TableStorage;
using Bertoni.TestFinale.Activity.Domain;
using Bertoni.TestFinale.Activity.Domain.Contracts;
using Bertoni.TestFinale.Domain;
using Bertoni.TestFinale.Domain.Contracts;
using Bertoni.TestFinale.Utils.AzureStorage.QueueStorage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bertoni.TestFinale.WebApi.Controllers
{
    [Authorize]
    public class ActivityApiController : ApiController
    {
        private IActivityRepository _repository;
        private ILogRepository _logRepository;

        public ActivityApiController() : base()
        {
            string storageConnection = ConfigurationManager.ConnectionStrings["azureStorageConnection"]
                .ConnectionString;
            
            _repository = new TableStorageRepository(storageConnection);
            _logRepository = new QueueStorageLogRepository(storageConnection);
        }

        // GET: api/ActivityApi
        public IEnumerable<ActivityDTO> Get()
        {
            return _repository.GetAll(User.Identity.Name);
        }

        // GET: api/ActivityApi/5
        public ActivityDTO Get(string id)
        {
            return _repository.GetById(User.Identity.Name, id);
        }

        // POST: api/ActivityApi
        public void Post([FromBody]ActivityDTO activity)
        {
            activity.Id = Guid.NewGuid();
            activity.CreationDate = DateTime.Now;

            string categoryDescription = null;
            try
            {
                categoryDescription = ActivityCategory.GetCategoryDesctiption(activity.CategoryId);
            }
            catch
            {
                categoryDescription = ActivityCategory.GetCategoryDesctiption(0);
            }

            activity.CategoryDescription = categoryDescription;
            _repository.Insert(User.Identity.Name, activity);
            _logRepository.LogToRepository(String.Format("User {0} INSERTED {1}",
                User.Identity.Name, JsonConvert.SerializeObject(activity)));
        }

        // PUT: api/ActivityApi/5
        public void Put(int id, [FromBody]ActivityDTO activity)
        {
            _repository.Update(User.Identity.Name, activity);
            _logRepository.LogToRepository(String.Format("User {0} UPDATED {1}",
                User.Identity.Name, JsonConvert.SerializeObject(activity)));
        }

        // DELETE: api/ActivityApi/5
        public void Delete(string id)
        {
            _repository.DeleteById(User.Identity.Name, id);
            _logRepository.LogToRepository(String.Format("User {0} DELETED id {1}", User.Identity.Name, id));
        }
    }
}
