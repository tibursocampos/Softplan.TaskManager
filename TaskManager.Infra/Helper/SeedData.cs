using TaskManager.Core.Entities;

namespace TaskManager.Infra.Helper;

public static class SeedData
{
    public static void Initialize(TaskManagerContext context)
    {
        if (context.Tasks.Any()) return;
        var now = DateTime.UtcNow;

        var firstUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var firstUserFirstTask = TaskItem.CreateForSeed("Implementar API", "Criar endpoints para task manager", now.AddDays(7), firstUserId);
        var firstUserSecondTask = TaskItem.CreateForSeed("Configurar Banco", "Definir modelos de dados", now.AddDays(5), firstUserId);
        var firstUserThirdTask = TaskItem.CreateForSeed("Testes Unitários", "Implementar suite de testes", now.AddDays(3), firstUserId);
        var firstUserFourthTask = TaskItem.CreateForSeed("Deploy Inicial", "Configurar ambiente produção", now.AddDays(1), firstUserId);

        var secondUserId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var secondUserFirstTask = TaskItem.CreateForSeed("Documentação", "Escrever guia de integração", now.AddDays(2), secondUserId);
        var secondUserSecondTask = TaskItem.CreateForSeed("Revisão Código", "Analisar PRs da equipe", now.AddDays(4), secondUserId);
        var secondUserThirdTask = TaskItem.CreateForSeed("Otimizar Performance", "Identificar e corrigir gargalos no sistema", now.AddDays(6), secondUserId);

        var thirdUserId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var thirdUserFirstTask = TaskItem.CreateForSeed("Monitoramento", "Configurar métricas", now.AddDays(1), thirdUserId);
        var thirdUserSecondTask = TaskItem.CreateForSeed("Configurar CI/CD", "Implementar pipeline de deploy automático", now.AddDays(2), thirdUserId);

        context.Tasks.AddRange(
            firstUserFirstTask,
            firstUserSecondTask,
            firstUserThirdTask,
            firstUserFourthTask,
            secondUserFirstTask,
            secondUserThirdTask,
            secondUserSecondTask,
            thirdUserFirstTask,
            thirdUserSecondTask
        );

        context.SaveChanges();
    }
}
