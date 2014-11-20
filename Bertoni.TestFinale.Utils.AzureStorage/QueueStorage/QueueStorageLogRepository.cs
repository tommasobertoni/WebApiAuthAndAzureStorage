using Bertoni.TestFinale.Activity.Domain.Contracts;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bertoni.TestFinale.Utils.AzureStorage.QueueStorage
{
    public class QueueStorageLogRepository : ILogRepository
    {
        private CloudQueue _cloudQueue;

        public QueueStorageLogRepository(string storageConnection)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnection);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            _cloudQueue = queueClient.GetQueueReference("logrepository");
            _cloudQueue.CreateIfNotExists();
        }

        void ILogRepository.LogToRepository(string log)
        {
            _cloudQueue.AddMessage(new CloudQueueMessage(log));
        }
    }
}
