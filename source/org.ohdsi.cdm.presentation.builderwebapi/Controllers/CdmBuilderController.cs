﻿using System;
using System.Data.Odbc;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using org.ohdsi.cdm.presentation.builderwebapi.Hubs;

namespace org.ohdsi.cdm.presentation.builderwebapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class CdmBuilderController : ControllerBase
    {
        private readonly IHubContext<LogHub> _logHub;
        private readonly IBackgroundTaskQueue _queue;
        private IConfiguration _configuration;

        public CdmBuilderController(IConfiguration configuration, IHubContext<LogHub> hub, IBackgroundTaskQueue queue)
        {
            _logHub = hub;
            _queue = queue;
            _configuration = configuration;
        }

        [HttpGet]
        public string Get()
        {
            return _queue.State;
        }


        [HttpGet("abort")]
        public string Abort()
        {
            _queue.Aborted = true;
            _queue.State = "Aborted";
            WriteLog("Aborted");
            return "Aborted";
        }

        [HttpPost("checksourceconnection")]
        public IActionResult CheckSourceConnection([FromBody] ConversionSettings settings)
        {
            try
            {
                ChekConnectionString(settings.SourceEngine,
                                     settings.SourceServer,
                                     settings.SourceDatabase,
                                     settings.SourceUser,
                                     settings.SourcePassword);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e);
            }

            return Ok();
        }
                
        [HttpPost("checkdestinationconnection")]
        public IActionResult CheckDestinationConnection([FromBody] ConversionSettings settings)
        {
            try
            {
                ChekConnectionString(settings.DestinationEngine,
                                     settings.DestinationServer,
                                     settings.DestinationDatabase,
                                     settings.DestinationUser,
                                     settings.DestinationPassword);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e);
            }

            return Ok();
        }

        [HttpPost("checkvocabularyconnection")]
        public IActionResult CheckVocabularyConnection([FromBody] ConversionSettings settings)
        {
            try
            {
                ChekConnectionString(settings.VocabularyEngine,
                                     settings.VocabularyServer,
                                     settings.VocabularyDatabase,
                                     settings.VocabularyUser,
                                     settings.VocabularyPassword);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e);
            }

            return Ok();
        }

        private void ChekConnectionString(string dbType, string server, string db, string user, string pswd)
        {
            var connection = _configuration[dbType].Replace("{server}", server)
                                                   .Replace("{database}", db)
                                                   .Replace("{username}", user)
                                                   .Replace("{password}", pswd);

            using var c = new OdbcConnection(connection);
            c.Open();
        }

        [HttpPost("vocabularyversion")]
        public string GetVocabularyVersion([FromBody] ConversionSettings settings)
        {
            const string query = "SELECT VOCABULARY_VERSION FROM vocabulary WHERE VOCABULARY_ID = 'None'";

            try
            {
                var cs = _configuration[settings.VocabularyEngine]
                    .Replace("{server}", settings.VocabularyServer)
                    .Replace("{database}", settings.VocabularyDatabase)
                    .Replace("{username}", settings.VocabularyUser)
                    .Replace("{password}", settings.VocabularyPassword);

                using var c = new OdbcConnection(cs);
                using var cmd = new OdbcCommand(query, c);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            catch (Exception)
            {
                return "Unknown";
            }

            return "Unknown";
        }

        [HttpPost("addmappings")]
        public async Task<IActionResult> AddMappings([FromForm] Mappings mappings)
        {
            var dir = Path.Combine(AppContext.BaseDirectory, "mappings");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var filePath = Path.Combine(dir, mappings.Name + ".zip");

            using (var stream = System.IO.File.Create(filePath))
            {
                await mappings.File.CopyToAsync(stream);
            }

            return Ok();
        }


        [HttpPost]
        public async Task<HttpResponseMessage> Post(CancellationToken cancellationToken, [FromBody] ConversionSettings settings)
        {
            HttpResponseMessage returnMessage = new HttpResponseMessage();
            WriteLog("conversion added to ");

            _queue.QueueBackgroundWorkItem(async token =>
            {
                await Task.Run(() =>
                {
                    _queue.State = "Running";

                    var conversion = new ConversionController(_queue, settings, _configuration, _logHub);
                    conversion.Start();

                    _queue.State = "Idle";
                });
            });

            //WriteLog("conversion done");
            return await Task.FromResult(returnMessage);
        }

        private void WriteLog(string message)
        {
            _logHub.Clients.All.SendAsync("Log", string.Format("{0}| {1}", DateTime.Now, message)).Wait();
        }
    }   
}
