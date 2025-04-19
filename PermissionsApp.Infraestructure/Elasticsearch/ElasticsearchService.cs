using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using PermissionsApp.Domain.Entities;
using PermissionsApp.Domain.Interfaces;

namespace PermissionsApp.Infraestructure.Elasticsearch
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly ElasticsearchClient _elasticClient;
        private readonly string _indexName;

        public ElasticsearchService(ElasticsearchClient elasticClient, string indexName)
        {
            _elasticClient = elasticClient;
            _indexName = indexName;
        }

        public async Task InitializeIndexAsync()
        {
            var indexExists = await _elasticClient.Indices.ExistsAsync(_indexName);

            if (!indexExists.Exists)
            {
                var createIndexResponse = await _elasticClient.Indices.CreateAsync(_indexName);

                if (!createIndexResponse.IsValidResponse)
                {
                    throw new Exception($"Failed to create index: {createIndexResponse.DebugInformation}");
                }

                Console.WriteLine($"The index {_indexName} has been created sucessfully.");
            } else {
                Console.WriteLine($"The index {_indexName} already exists.");
            }
        }

        public async Task IndexPermissionAsync(Permission permission)
        {
            await _elasticClient.IndexAsync(permission, _indexName);
        }

        public async Task<IEnumerable<Permission>> SearchPermissionsAsync(string searchTerm)
        {
            var searchResponse = await _elasticClient.SearchAsync<Permission>(s => s
                .Indices(_indexName)
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(new[] { "employeeName", "employeeLastName" })
                        .Query(searchTerm)
                    )
                )
            );

            return searchResponse.Documents;
        }
    }
}
