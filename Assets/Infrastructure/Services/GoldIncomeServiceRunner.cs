using UnityEngine;
using VContainer;
using CityBuilder.Application.Services;

namespace CityBuilder.Infrastructure.Services
{
    public class GoldIncomeServiceRunner : MonoBehaviour
    {
        private GoldIncomeService? _goldIncomeService;

        [Inject]
        public void Construct(GoldIncomeService goldIncomeService)
        {
            _goldIncomeService = goldIncomeService;
        }

        private void Start()
        {
            _goldIncomeService?.Start();
        }

        private void OnDestroy()
        {
            _goldIncomeService?.Stop();
        }
    }
}
