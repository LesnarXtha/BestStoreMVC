using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BankTransaction.Models;
using System.Text;

namespace BankTransaction.Controllers
{
    public class TransactionController : Controller
    {
        private readonly TransactionDbContext _context;

        public TransactionController(TransactionDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index(string searchQuery)
        {
            ViewData["CurrentFilter"] = searchQuery;

            var transactions = from t in _context.Transactions select t;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                transactions = transactions.Where(t => t.BeneficiaryName.Contains(searchQuery)
                                                    || t.AccountNumber.Contains(searchQuery)
                                                    || t.Tags.Contains(searchQuery)); // Include Tags in search
            }

            return View(await transactions.ToListAsync());
        }

        // GET: Transaction/AddOrEdit
        public IActionResult AddOrEdit(int id=0)
        {
            if(id == 0) 
            {
                return View(new Transaction()); 
            }
            else
            {
                return View(_context.Transactions.Find(id));
            }
            
        }

        //
        // : Transaction/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,AccountNumber,BeneficiaryName,BankName,SwiftCode,Amount,Date,Tags")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.TransactionId == 0)
                {
                    transaction.Date = DateTime.Now;
                    _context.Add(transaction); // Add a new transaction
                }
                else
                {
                    _context.Update(transaction); // Update an existing transaction
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }


        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ExportToCsv(string searchQuery)
        {
            // Filter the transactions based on the search query
            var transactions = from t in _context.Transactions select t;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                transactions = transactions.Where(t => t.BeneficiaryName.Contains(searchQuery)
                                                    || t.AccountNumber.Contains(searchQuery)
                                                    || t.Tags.Contains(searchQuery));
            }

            var filteredTransactions = await transactions.ToListAsync();

            // Create the CSV content
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("TransactionId,AccountNumber,BeneficiaryName,BankName,SwiftCode,Tags,Amount,Date");

            foreach (var transaction in filteredTransactions)
            {
                csvBuilder.AppendLine(
                    $"{transaction.TransactionId}," +
                    $"{transaction.AccountNumber}," +
                    $"{transaction.BeneficiaryName}," +
                    $"{transaction.BankName}," +
                    $"{transaction.SwiftCode}," +
                    $"{transaction.Tags}," +
                    $"{transaction.Amount}," +
                    $"{transaction.Date:MMM-dd-yy}"
                );
            }

            // Return the CSV file
            return File(Encoding.UTF8.GetBytes(csvBuilder.ToString()), "text/csv", "FilteredTransactions.csv");
        }


        [HttpPost]
        public async Task<IActionResult> ImportFromCsv(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                ModelState.AddModelError("", "Please select a valid CSV file.");
                return RedirectToAction(nameof(Index));
            }

            using (var stream = new StreamReader(csvFile.OpenReadStream()))
            {
                var lineNumber = 0;
                while (!stream.EndOfStream)
                {
                    var line = await stream.ReadLineAsync();
                    if (lineNumber == 0) // Skip header row
                    {
                        lineNumber++;
                        continue;
                    }

                    var values = line.Split(',');
                    var transaction = new Transaction
                    {
                        AccountNumber = values[1],
                        BeneficiaryName = values[2],
                        BankName = values[3],
                        SwiftCode = values[4],
                        Tags = values[5], // Import Tags column
                        Amount = int.Parse(values[6]),
                        Date = DateTime.Parse(values[7])
                    };

                    _context.Transactions.Add(transaction);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionId == id);
        }
    }
}
