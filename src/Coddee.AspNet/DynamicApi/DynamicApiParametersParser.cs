using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coddee.AspNet
{
    /// <summary>
    /// Parses the request parameters.
    /// </summary>
    public class DynamicApiParametersParser
    {
        private readonly IsoDateTimeConverter _dateTimeConverter;
        private readonly JsonSerializerSettings _jsonSerializer;

        /// <inheritdoc />
        public DynamicApiParametersParser(IsoDateTimeConverter dateTimeConverter)
        {
            _dateTimeConverter = dateTimeConverter;
            _jsonSerializer = new JsonSerializerSettings
            {
                DateFormatString = _dateTimeConverter.DateTimeFormat
            };
        }

        /// <summary>
        /// Parse request parameters.
        /// </summary>
        public async Task<DynamicApiActionParameterValue[]> ParseParameters(IDynamicApiAction action, DynamicApiRequest request)
        {
            var parameters = new DynamicApiActionParameterValue[action.Parameters.Count];
            var httpRequest = request.HttpContext.Request;


            for (int i = 0; i < action.Parameters.Count; i++)
            {
                var actionParameter = action.Parameters[i];
                bool found = false;
                if (HttpMethod.IsGet(httpRequest.Method) || HttpMethod.IsDelete(httpRequest.Method))
                {

                    if (httpRequest.Query.ContainsKey(actionParameter.Name))
                    {
                        var value = ParseGetValue(actionParameter, httpRequest.Query[actionParameter.Name]);
                        var paramValue = new DynamicApiActionParameterValue
                        {
                            Parameter = actionParameter,
                            Value = value
                        };
                        parameters[i] = paramValue;
                        found = true;
                    }
                }
                else
                {
                    string content;
                    using (var sr = new StreamReader(httpRequest.Body))
                    {
                        content = await sr.ReadToEndAsync();
                    }
                    var value = ParsePostValue(actionParameter, content);
                    var paramValue = new DynamicApiActionParameterValue
                    {
                        Parameter = actionParameter,
                        Value = value
                    };
                    parameters[i] = paramValue;
                    found = true;
                }

                if (!found)
                    throw new DynamicApiException(DynamicApiExceptionCodes.MissingParameter, $"Parameter ({actionParameter.Name}) is missing.", request);
            }



            return parameters;
        }

        private object ParsePostValue(DynamicApiActionParameter actionParameter, string content)
        {
            return JsonConvert.DeserializeObject(content, actionParameter.Type, _jsonSerializer);
        }

        private object ParseGetValue(DynamicApiActionParameter actionParameter, StringValues queryParam)
        {
            try
            {
                if (actionParameter.Type == typeof(DateTime))
                    return DateTime.ParseExact(queryParam.ToString(), _dateTimeConverter.DateTimeFormat, null);

                var converter = TypeDescriptor.GetConverter(actionParameter.Type);
                return converter.ConvertFrom(queryParam.ToString());
            }
            catch (NotSupportedException)
            {
                return JsonConvert.DeserializeObject(queryParam.ToString(), actionParameter.Type, _jsonSerializer);
            }
        }
    }
}