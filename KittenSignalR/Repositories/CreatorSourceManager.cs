using KittenSignalR.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace KittenSignalR.Repositories
{
    public class CreatorSourceManager : ICreatorSourceManager
    {
        private List<Creator> creatorList;

        public CreatorSourceManager()
        {
            this.creatorList = GetCreators();
        }

        public List<Creator> GetCreators()
        {
            //add console logigng that we are loading the list
            Console.WriteLine("Loading creator list");
            var jsonContent = System.IO.File.ReadAllText("creatorList.json");

            // Deserialize the JSON into a List<Channel>
            var creators = JsonSerializer.Deserialize<List<Creator>>(jsonContent);

            return creators;
        }

        public List<Creator> SetNewCreator(Creator creator)
        {
            Console.WriteLine("Updating the creator list");

            //take in new creator item and convert it to json, add to list and then save it
            creatorList.Add(creator);
            //we need to use this property when serializing WriteIndented = true
            var serializerOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonContent = JsonSerializer.Serialize(creatorList, serializerOptions);

            System.IO.File.WriteAllText("creatorList.json", jsonContent);

            return creatorList;
        }
    }
}