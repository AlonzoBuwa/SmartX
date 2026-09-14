using SmartX.Api.Models;

namespace SmartX.Api.Services
{
    public class DeploymentValidator
    {
        public bool ValidateNode(DeploymentNode node)
        {
            if (!node.IsConfigured)
            {
                return false;
            }

            foreach (var child in node.Children)
            {
                if (!ValidateNode(child))
                {
                    return false;
                }
            }

            return true;
        }
    }
}