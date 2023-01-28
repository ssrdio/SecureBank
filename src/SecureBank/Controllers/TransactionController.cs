using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureBank.Helpers.Authorization.Attributes;
using SecureBank.DAL.DBModels;
using SecureBank.DAL;
using SecureBank.Interfaces;
using SecureBank.Helpers;
using SecureBank.Filters;

namespace SecureBank.Controllers
{
    [AuthorizeNormal(AuthorizeAttributeTypes.Mvc)]

    public class TransactionController : MvcBaseContoller
    {
        private readonly PortalDBContext _context;

        private readonly ITransactionBL _transactionBL;

        public TransactionController(PortalDBContext context, ITransactionBL transactionBL)
        {
            _context = context;

            _transactionBL = transactionBL;
        }

        // GET: Transaction
        public IActionResult Index()
        {
            string viewName = _transactionBL.GetIndexViewName();

            return View(viewName);
        }

        // GET: Transaction/Details/5
        public IActionResult Details(int? id)
        {
            TransactionDBModel transaction = _transactionBL.Details(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transaction/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Transaction/Create
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,SenderId,ReceiverId,TransactionDateTime,Reason,Amount,Reference")] TransactionDBModel transaction)
        {
            if (!ModelState.IsValid)
            {
                return View(transaction);
            }

            bool createResult = _transactionBL.Create(transaction);
            if (!createResult)
            {
                ModelState.AddModelError(string.Empty, "Error");
                return View(transaction);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Transaction/Edit/5
        [UnknownGeneration]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactionTable = await _context.Transactions.FindAsync(id);
            if (transactionTable == null)
            {
                return NotFound();
            }

            return View(transactionTable);
        }

        // POST: Transaction/Edit/5
        // To protect from overpoting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [UnknownGeneration]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SenderId,ReceiverId,TransactionDateTime,Reason,Amount,Reference")] TransactionDBModel transactionTable)
        {

            if (id != transactionTable.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transactionTable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionTableExists(transactionTable.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(transactionTable);
        }

        [UnknownGeneration]
        // GET: Transaction/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactionTable = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transactionTable == null)
            {
                return NotFound();
            }

            return View(transactionTable);
        }

        // POST: Transaction/Delete/5
        [UnknownGeneration]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transactionTable = await _context.Transactions.FindAsync(id);
            _context.Transactions.Remove(transactionTable);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionTableExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
