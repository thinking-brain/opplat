// using ContratacionWebApi.Data;
// using ContratacionWebApi.Models;
// using WorkflowCore.Interface;
// using WorkflowCore.Models;

// namespace ContratacionWebApi.Workflows
// {
//     public class AgregarContrato : IWorkflow<Contrato>
//     {
//         public string Id => "AgregarContratoWorkflow";

//         public int Version => 1;

//         public void Build(IWorkflowBuilder<Contrato> builder)
//         {

//         }
//     }

//     public class IniciarContrato : StepBody
//     {
//         public ContratacionDbContext _db { get; set; }

//         public Contrato Contrato { get; set; }

//         public IniciarContrato(ContratacionDbContext context)
//         {
//             _db = context;
//         }

//         public override ExecutionResult Run(IStepExecutionContext context)
//         {

//             _db.Add(Contrato);
//             _db.SaveChanges();
//             return ExecutionResult.Next();
//         }
//     }
// }