using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using ToDoListAPI.Model;
using ToDoListAPI.Services;

namespace ToDoListAPI.Functions.v1
{
    public class ToDoListFunction
    {
        private readonly ToDoListService _service;

        public ToDoListFunction(ToDoListService service)
        {
            _service = service;
        }

        [Function("CreateChecklist")]
        [OpenApiOperation(operationId: "CreateChecklist", tags: new[] { "Checklist" })]
        [OpenApiRequestBody("application/json", typeof(ToDoListPayload), Description = "Checklist data")]
        [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json", typeof(ToDoList), Description = "Checklist created")]
        public async Task<HttpResponseData> CreateChecklist(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "todoList/create")] HttpRequestData req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<ToDoListPayload>(requestBody);
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
                return req.CreateResponse(HttpStatusCode.BadRequest);

            var checklist = _service.AddList(new ToDoList { Id = Guid.NewGuid(), Name = request.Name });

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(checklist);
            return response;
        }

        [Function("GetChecklists")]
        [OpenApiOperation(operationId: "GetChecklists", tags: new[] { "Checklist" })]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(List<ToDoList>), Description = "List of checklists")]
        public async Task<HttpResponseData> GetChecklists(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "todoList/list")] HttpRequestData req)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(_service.GetLists());
            return response;
        }

        [Function("DeleteChecklist")]
        [OpenApiOperation(operationId: "DeleteChecklist", tags: new[] { "Checklist" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "Checklist ID")]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent, Description = "Checklist deleted")]
        public HttpResponseData DeleteChecklist(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "todoList/delete/{id}")] HttpRequestData req, Guid id)
        {
            return _service.DeleteList(id)
                ? req.CreateResponse(HttpStatusCode.NoContent)
                : req.CreateResponse(HttpStatusCode.NotFound);
        }

        [Function("AddItemToChecklist")]
        [OpenApiOperation(operationId: "AddItemToChecklist", tags: new[] { "Checklist Items" })]
        [OpenApiRequestBody("application/json", typeof(ToDoListItemPayload), Description = "Item data")]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ToDoList), Description = "Item added to checklist")]
        public async Task<HttpResponseData> AddItemToChecklist(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "todoList/add-item/{id}")] HttpRequestData req, Guid id)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<ToDoListItemPayload>(requestBody);
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
                return req.CreateResponse(HttpStatusCode.BadRequest);

            var checklist = _service.AddItemToList(id, new ToDoListItem { Id = Guid.NewGuid(), Name = request.Name });
            if (checklist == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(checklist);
            return response;
        }

        [Function("UpdateItemInChecklist")]
        [OpenApiOperation(operationId: "UpdateItemInChecklist", tags: new[] { "Checklist Items" })]
        [OpenApiRequestBody("application/json", typeof(ToDoListItemPayload), Description = "Updated item data")]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ToDoList), Description = "Item updated")]
        public async Task<HttpResponseData> UpdateItemInChecklist(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "todoList/update-item/{checklistId}/{itemId}")] HttpRequestData req, Guid checklistId, Guid itemId)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<ToDoListItemPayload>(requestBody);
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
                return req.CreateResponse(HttpStatusCode.BadRequest);

            var checklist = _service.UpdateItemInList(checklistId, itemId, request.Name);
            if (checklist == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(checklist);
            return response;
        }

    }
}
