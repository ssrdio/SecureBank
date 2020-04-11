using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using SecureBank.Ctf.CTFd.Models;
using SecureBank.Ctf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureBank.Ctf.CTFd
{
    public class CTFdExportService
    {
        private const string ALEMBIC_VERSION_2_2_0 = "080d29b15cd3";

        private const int CTFD_CHALLANGE_VALUE = 10;

        private const string CTFD_CHALLANGE_TYPE = "standard";
        private const string CTFD_CHALLANGE_STATE = "visible";

        private const string CTFD_FLAG_TYPE = "static";

        private const string ALEMBIC_VERSION_ENTRY_NAME = "db/alembic_version.json";
        private const string CHALLENGES_ENTRY_NAME = "db/challenges.json";
        private const string FLAGS_ENTRY_NAME = "db/flags.json";

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private CTFdBaseModel<CTFdAlembicVersionModel> GetAlembicVersion()
        {
            CTFdAlembicVersionModel cTFdAlembicVersion = new CTFdAlembicVersionModel(
                versionNum: ALEMBIC_VERSION_2_2_0);

            return new CTFdBaseModel<CTFdAlembicVersionModel>(cTFdAlembicVersion);
        }

        private CTFdBaseModel<CTFdChallengeModel> GetChallanges(List<CtfChallangeModel> ctfChallanges)
        {
            List<CTFdChallengeModel> cTFdChallanges = ctfChallanges
                .Select(x => new CTFdChallengeModel(
                    id: (int)x.Type,
                    name: x.Title,
                    description: "",
                    maxAttempts: 0,
                    value: CTFD_CHALLANGE_VALUE,
                    category: x.Category.ToString(),
                    type: CTFD_CHALLANGE_TYPE,
                    state: CTFD_CHALLANGE_STATE))
                .ToList();

            return new CTFdBaseModel<CTFdChallengeModel>(cTFdChallanges);
        }

        private CTFdBaseModel<CTFdFlagModel> GetFlags(List<CtfChallangeModel> ctfChallanges)
        {
            int flagIndex = 1;

            List<CTFdFlagModel> cTFdFlags = ctfChallanges
                .Select(x => new CTFdFlagModel(
                    id: flagIndex++,
                    challengeId: (int)x.Type,
                    type: CTFD_FLAG_TYPE,
                    content: x.Flag,
                    data: null))
                .ToList();

            return new CTFdBaseModel<CTFdFlagModel>(cTFdFlags);
        }

        public async Task Export(string path, List<CtfChallangeModel> ctfChallanges)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to create folder for ctf. {ex.Message}");
                    return;
                }
            }

            string exportFile = Path.Combine(path, $"{Guid.NewGuid()}.zip");

            using FileStream fileStream = new FileStream(exportFile, FileMode.CreateNew);
            using ZipArchive zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Create, false);

            CTFdBaseModel<CTFdAlembicVersionModel> alembicVersion = GetAlembicVersion();
            await WriteZipDbEntry(zipArchive, ALEMBIC_VERSION_ENTRY_NAME, alembicVersion);

            CTFdBaseModel<CTFdChallengeModel> challanges = GetChallanges(ctfChallanges);
            await WriteZipDbEntry(zipArchive, CHALLENGES_ENTRY_NAME, challanges);

            CTFdBaseModel<CTFdFlagModel> flags = GetFlags(ctfChallanges);
            await WriteZipDbEntry(zipArchive, FLAGS_ENTRY_NAME, flags);
        }

        private static async Task WriteZipDbEntry<T>(ZipArchive zipArchive, string entryName, CTFdBaseModel<T> model)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy(),
                },
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            };

            using Stream zipStream = zipArchive.CreateEntry(entryName, CompressionLevel.Fastest).Open();

            string json = JsonConvert.SerializeObject(model, jsonSerializerSettings);
            byte[] data = Encoding.UTF8.GetBytes(json);

            await zipStream.WriteAsync(data, 0, data.Length);
        }
    }
}
