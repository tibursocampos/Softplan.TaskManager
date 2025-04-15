using Microsoft.EntityFrameworkCore;

using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Infra.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagerContext _context;

    public TaskRepository(TaskManagerContext context)
    {
        _context = context;
    }

    public async Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return task;
    }

    public async Task<IEnumerable<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool trackEntity = false)
    {
        var query = _context.Tasks.Where(t => t.Id == id);
        return await (trackEntity ? query : query.AsNoTracking()).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
