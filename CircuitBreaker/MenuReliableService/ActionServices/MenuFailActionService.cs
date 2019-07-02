using CircuitBreaker.Contract.ReliableService.Models;
using System.Collections.Generic;

namespace MenuReliableService.ActionServices
{
    public class MenuFailActionService
    {
        public void InvokeGet(string id, Response<Menu> result)
        {
            result.Data = new Menu {
                Description = "Test Fail Des",
                Name = "Test Fail",
            };
        }

        public void InvokeGet(Response<IEnumerable<Menu>> result)
        {
            result.Data = new List<Menu>()
            {
                new Menu
                {
                    Description = "Test Fail Des",
                    Name = "Test Fail",
                }
            };
        }
    }
}
