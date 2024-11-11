using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IAController : ControllerBase
    {
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _model;

        public IAController( PredictionEnginePool<ModelInput,ModelOutput> predictionEnginePool) 
        { 
            _model = predictionEnginePool;
        }

        [HttpGet]
        public ModelOutput Predict(string text)
        {
            ModelInput input = new ModelInput
            {
                Text = text
            };

            ModelOutput output = _model.Predict(input);

            return output;
        }
    }
}
