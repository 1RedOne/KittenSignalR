using KittenSignalR.Models;
using System.Collections.Generic;

namespace KittenSignalR
{
    public interface ICreatorSourceManager
    {
        List<Creator> GetCreators();

        List<Creator> SetNewCreator(Creator creator);

        void DeleteCreator(Creator creator);

        void RefreshCreator(Creator creator);
    }
}