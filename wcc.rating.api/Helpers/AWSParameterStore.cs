using Amazon;
using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace wcc.rating.api.Helpers
{
    public class AWSParameterStore
    {
        private static AWSParameterStore _instance;
        private static readonly object _lock = new object();
        private readonly IAmazonSimpleSystemsManagement _ssmClient;

        private AWSParameterStore(IAmazonSimpleSystemsManagement ssmClient)
        {
            _ssmClient = ssmClient;
        }

        public static AWSParameterStore Instance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        var aws_access_key = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
                        var aws_secret_key = Environment.GetEnvironmentVariable("AWS_SECRET_KEY");

                        var credentials = new BasicAWSCredentials(aws_access_key, aws_secret_key);
                        var ssmClient = new AmazonSimpleSystemsManagementClient(
                                    credentials, RegionEndpoint.EUCentral1);

                        _instance = new AWSParameterStore(ssmClient);
                    }
                }
            }
            return _instance;
        }

        public async Task<string> GetParameterAsync(string parameterName)
        {
            var request = new GetParameterRequest
            {
                Name = parameterName,
                WithDecryption = true
            };

            var response = await _ssmClient.GetParameterAsync(request);
            return response.Parameter.Value;
        }

        public async Task<Dictionary<string, string>> GetParametersByPathAsync(string path)
        {
            var request = new GetParametersByPathRequest
            {
                Path = path,
                Recursive = true,
                WithDecryption = true
            };

            var response = await _ssmClient.GetParametersByPathAsync(request);
            var parameters = new Dictionary<string, string>();

            foreach (var parameter in response.Parameters)
            {
                parameters.Add(parameter.Name, parameter.Value);
            }

            return parameters;
        }
    }
}
