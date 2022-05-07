﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Data;
using SmartLearning.Models;

namespace SmartLearning.Controllers
{
  [Authorize(Roles = "Admin")]
  public class ClassProposalController : Controller
  {
    private readonly ApplicationDbContext _context;

    public ClassProposalController(ApplicationDbContext context)
    {
      _context = context;
    }

    // GET: ClassProposals
    public async Task<IActionResult> Index()
    {
      var applicationDbContext = _context.ClassProposals.Include(c => c.Board).Include(c => c.Standard).Include(c => c.Subject);
      return View(await applicationDbContext.ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(string id)
    {
      var classProposal = await _context.ClassProposals.FindAsync(id);
      await _context.Classes.AddAsync(new Class { BoardId = classProposal.BoardId, SubjectId = classProposal.SubjectId, StandardId = classProposal.StandardId });
      _context.ClassProposals.Remove(classProposal);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(string id)
    {
      var classProposal = await _context.ClassProposals.FindAsync(id);
      _context.ClassProposals.Remove(classProposal);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool ClassProposalExists(string id)
    {
      return _context.ClassProposals.Any(e => e.Id == id);
    }
  }
}
