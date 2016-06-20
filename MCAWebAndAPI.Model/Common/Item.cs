using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.Common
{
    public abstract class Item
    {
        [UIHint("Int32")]
        public int? ID { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// To detect if any property has been changed (when editing in kendo grid)
        /// </summary>
        [DisplayName("Action")]
        [UIHint("Int32")]
        public int EditMode { get; set; } = (int)Mode.CREATED;

        public enum Mode
        {
            DELETED = -1,
            UPDATED = 1,
            CREATED = 0
        }

        public static bool CheckIfSkipped(Item item)
        {
            // If existing item and not updated nor deleted, then skipped
            if (item.ID != null &&
                   !(item.EditMode == (int)Item.Mode.UPDATED || item.EditMode == (int)Mode.DELETED))
                return true;

            // If new item and deleted
            if (item.ID == null && item.EditMode == (int)Mode.DELETED)
                return true;

            return false;
        }

        public static bool CheckIfDeleted(Item item)
        {
            return item.EditMode == (int)Mode.DELETED;
        }

        public static bool CheckIfUpdated(Item item)
        {
            return item.EditMode == (int)Mode.UPDATED;
        }

        public static bool CheckIfCreated(Item item)
        {
            return item.EditMode == (int)Mode.CREATED;
        }

    }
}
