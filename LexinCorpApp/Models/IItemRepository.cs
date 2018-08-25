using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IItemRepository
    {
        IQueryable<Item> Items { get; }
        void Save(Item item);
    }
}
