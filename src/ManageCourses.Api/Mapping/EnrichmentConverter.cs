using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using Newtonsoft.Json;

namespace GovUk.Education.ManageCourses.Api.Mapping
{
    public class EnrichmentConverter
    {
        private JsonSerializerSettings _jsonSerializerSettings;

        public EnrichmentConverter()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
        }        

        public string ConvertToJson(UcasProviderEnrichmentPostModel model)
        {
            return JsonConvert.SerializeObject(model.EnrichmentModel, _jsonSerializerSettings);
        }
        
        public string ConvertToJson(CourseEnrichmentModel model)
        {
            return JsonConvert.SerializeObject(model, _jsonSerializerSettings);
        }
        
        /// <summary>
        /// maps enrichment data from the data object to the returned enrichment model
        /// </summary>
        /// <param name="source">enrichment data object</param>
        /// <returns>enrichment model with enrichment data (if found). Null if there is no source record</returns>
        public UcasCourseEnrichmentGetModel Convert(CourseEnrichment source)
        {
            if (source == null) return null;

            var enrichmentToReturn = new UcasCourseEnrichmentGetModel();
            var enrichmentModel = source.JsonData != null ? JsonConvert.DeserializeObject<CourseEnrichmentModel>(source.JsonData, _jsonSerializerSettings) : null;

            enrichmentToReturn.EnrichmentModel = enrichmentModel;
            enrichmentToReturn.CreatedTimestampUtc = source.CreatedTimestampUtc;
            enrichmentToReturn.UpdatedTimestampUtc = source.UpdatedTimestampUtc;
            enrichmentToReturn.CreatedByUserId = source.CreatedByUser?.Id ?? 0;
            enrichmentToReturn.UpdatedByUserId = source.UpdatedByUser?.Id ?? 0;
            enrichmentToReturn.LastPublishedTimestampUtc = source.LastPublishedTimestampUtc;
            enrichmentToReturn.Status = source.Status;
            enrichmentToReturn.ProviderCode = source.ProviderCode;
            enrichmentToReturn.CourseCode = source.UcasCourseCode;
            return enrichmentToReturn;
        }

        /// <summary>
        /// maps enrichment data from the data object to the returned enrichment model
        /// </summary>
        /// <param name="source">enrichment data object</param>
        /// <returns>enrichment model with enrichment data (if found). Null if there is no source record</returns>
        public UcasProviderEnrichmentGetModel Convert(ProviderEnrichment source)
        {
            if (source == null) return null;

            var enrichmentToReturn = new UcasProviderEnrichmentGetModel();
            var enrichmentModel = source.JsonData != null ? JsonConvert.DeserializeObject<ProviderEnrichmentModel>(source.JsonData, _jsonSerializerSettings) : null;

            enrichmentToReturn.EnrichmentModel = enrichmentModel;
            enrichmentToReturn.CreatedTimestampUtc = source.CreatedAt;
            enrichmentToReturn.UpdatedTimestampUtc = source.UpdatedAt;
            enrichmentToReturn.CreatedByUserId = source.CreatedByUser.Id;
            enrichmentToReturn.UpdatedByUserId = source.UpdatedByUser.Id;
            enrichmentToReturn.LastPublishedTimestampUtc = source.LastPublishedAt;
            enrichmentToReturn.Status = source.Status;
            return enrichmentToReturn;
        }
    }
}
