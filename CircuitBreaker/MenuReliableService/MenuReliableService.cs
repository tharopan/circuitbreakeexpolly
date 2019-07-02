using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CircuitBreaker.Contract;
using CircuitBreaker.Contract.ReliableService;
using CircuitBreaker.Contract.ReliableService.MenuServices;
using CircuitBreaker.Contract.ReliableService.Models;
using MenuReliableService.ActionServices;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;

namespace MenuReliableService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class MenuReliableService : StatefulService, IMenuReliableService
    {
        private CircuitBreakerManager circuitBreaker;
        private MenuActionService menuActionService;
        private MenuFailActionService menuFailActionService;

        public MenuReliableService(StatefulServiceContext context)
            : base(context)
        {
            this.circuitBreaker = new CircuitBreakerManager(50000);
            this.menuActionService = new MenuActionService();
            this.menuFailActionService = new MenuFailActionService();
        }

        public async Task<Response<IEnumerable<Menu>>> Get()
        {
            Response<IEnumerable<Menu>> result = new Response<IEnumerable<Menu>>();

            var cts = new CancellationTokenSource();
            var token = cts.Token;
            token.ThrowIfCancellationRequested();

            var crcBreaker = this.circuitBreaker.Invoke(
                async () =>
                {
                    menuActionService.InvokeGet(result);
                },
                async () =>
                {
                    menuFailActionService.InvokeGet(result);
                });

            return result;
        }


        public async Task<Response<Menu>> Get(string id)
        {
            Response<Menu> result = new Response<Menu>();

            var cts = new CancellationTokenSource();
            var token = cts.Token;
            token.ThrowIfCancellationRequested();

            var crcBreaker = this.circuitBreaker.Invoke(
                async () =>
                {
                    menuActionService.InvokeGet(id, result);
                },
                async () =>
                {
                    menuFailActionService.InvokeGet(id, result);
                });

            return result;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[0];
        }
    }
}
