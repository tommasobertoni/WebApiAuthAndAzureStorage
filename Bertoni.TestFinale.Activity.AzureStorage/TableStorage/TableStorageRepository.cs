using Bertoni.TestFinale.Domain;
using Bertoni.TestFinale.Domain.Contracts;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bertoni.TestFinale.Activity.AzureStorage.TableStorage
{
    public class TableStorageRepository : IActivityRepository
    {
        private CloudTable _cloudTable;

        public TableStorageRepository(string storageConnection)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnection);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            _cloudTable = tableClient.GetTableReference("activities");
            _cloudTable.CreateIfNotExists();
        }

        IEnumerable<ActivityDTO> IActivityRepository.GetAll(string username)
        {
            var whereClause = String.Format("PartitionKey eq '{0}'", username);
            TableQuery<ActivityTableEntity> queryGetAll = new TableQuery<ActivityTableEntity>()
                .Where(whereClause);

            var activityTableEntities = _cloudTable.ExecuteQuery(queryGetAll)
                .OrderByDescending(ate => ate.CreationDate);

            return activityTableEntities.Select(
                ate => new ActivityDTO
                {
                    Id = ate.Id,
                    Title = ate.Title,
                    Desc = ate.Desc,
                    CreationDate = ate.CreationDate,
                    Duration = ate.Duration,
                    CategoryId = ate.CategoryId,
                    CategoryDescription = ate.CategoryDescription
                });
        }

        ActivityDTO IActivityRepository.GetById(string username, string id)
        {
            ActivityTableEntity ate = GetActivityTableEntityById(username, id);

            return new ActivityDTO
            {
                Id = ate.Id,
                Title = ate.Title,
                Desc = ate.Desc,
                CreationDate = ate.CreationDate,
                Duration = ate.Duration,
                CategoryId = ate.CategoryId,
                CategoryDescription = ate.CategoryDescription
            };
        }

        void IActivityRepository.Insert(string username, ActivityDTO activity)
        {
            ActivityTableEntity ate = (ActivityTableEntity) activity;
            ate.PartitionKey = username;
            ate.RowKey = ate.Id.ToString();

            TableOperation insert = TableOperation.Insert(ate);
            _cloudTable.Execute(insert);
        }

        void IActivityRepository.Update(string username, ActivityDTO activity)
        {
            if (activity.Id == null)
                throw new ArgumentException("Activity Id can't be null");

            ActivityTableEntity ate = GetActivityTableEntityById(username, activity.Id.ToString());

            ate.Title = activity.Title;
            ate.Desc = activity.Desc;
            ate.CreationDate = activity.CreationDate;
            ate.Duration = activity.Duration;
            ate.CategoryId = activity.CategoryId;
            ate.CategoryDescription = activity.CategoryDescription;

            TableOperation update = TableOperation.Replace(ate);
            _cloudTable.Execute(update);
        }

        void IActivityRepository.DeleteById(string username, string id)
        {
            ActivityTableEntity ate = GetActivityTableEntityById(username, id);

            TableOperation delete = TableOperation.Delete(ate);
            _cloudTable.Execute(delete);
        }

        private ActivityTableEntity GetActivityTableEntityById(string partitionKey, string rowKey)
        {
            return _cloudTable.Execute(
                TableOperation.Retrieve<ActivityTableEntity>(partitionKey, rowKey))
                .Result as ActivityTableEntity;
        }
    }

    public class ActivityTableEntity : TableEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Desc { get; set; }

        public DateTime CreationDate { get; set; }

        public int Duration { get; set; }

        public int CategoryId { get; set; }

        public string CategoryDescription { get; set; }

        public static explicit operator ActivityTableEntity(ActivityDTO activity)
        {
            return new ActivityTableEntity
            {
                Id = activity.Id,
                Title = activity.Title,
                Desc = activity.Desc,
                CreationDate = activity.CreationDate,
                Duration = activity.Duration,
                CategoryId = activity.CategoryId,
                CategoryDescription = activity.CategoryDescription
            };
        }
    }
}
