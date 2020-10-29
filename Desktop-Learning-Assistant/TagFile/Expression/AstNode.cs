using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Expression
{
    /// <summary>
    /// 语法树节点
    /// </summary>
    public abstract class AstNode
    {
        /// <summary>
        /// 获取以该节点为根的子树的求值结果
        /// </summary>
        public abstract bool Value(QueryFile file);
    }
}
