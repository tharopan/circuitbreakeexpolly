﻿using CircuitBreaker.Contract.ReliableService;
using CircuitBreaker.Contract.ReliableService.Models;
using RestSharp;
using System;
using System.Collections.Generic;

namespace MenuReliableService.ActionServices
{
    public class MenuActionService
    {
        public void InvokeGet(string id, Response<Menu> result)
        {
            var client = new RestClient("URLLLLLLL");
            var request = new RestRequest("?id={id}", Method.GET);
            request.AddUrlSegment("id", id);
            request.Timeout = TimeSpan.FromSeconds(10).Milliseconds;
            var response = client.Execute<Menu>(request);
            if (response?.Data != null)
            {
                result.Data = response.Data;
                result.CircuitState = CircuitState.Open;
            }
            else
            {
                throw new ApplicationException();
            }
        }

        public void InvokeGet(Response<IEnumerable<Menu>> result)
        {
            var client = new RestClient("URLLLLLLL");
            var request = new RestRequest("", Method.GET);
            request.Timeout = TimeSpan.FromSeconds(10).Milliseconds;
            var response = client.Execute<List<Menu>>(request);
            if (response?.Data != null)
            {
                result.Data = response.Data;
                result.CircuitState = CircuitState.Open;
            }
            else
            {
                throw new ApplicationException();
            }
        }
    }
}
