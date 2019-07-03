using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMetaphorWorld
{
    public class InstructionMacro
    {
        /// <summary>
        /// This list contains a series of indexes, they allow for the user to perform a for abstract action and then have all the associated fineGrained operations performed automatically.
        /// It will always start with zero, and no subsequent numbers can be equal to or smaller than previous indexes. If you where using no abstraction (i.e. finest grained control, then this
        /// will simply contain something like: {0,1,2,3,4,5,6}
        /// </summary>
        List<int> indexForStartOfInstructionSet;

        /// <summary>
        /// The finest grained instructions that need to be executed for a particular program trace
        /// eg {"declarevariable 'int x'", "newcalculatorexpression '2'", "evaluatecalculator", "assignhandtovariable 'x'"}
        /// would correspond to code such as this:
        /// int x = 2;
        /// OR
        /// int x;
        /// x = 2;
        /// </summary>
        List<string> fineGrainedInstructions;

        List<string> associatedHints;

        int currentIntructionSet;

        public InstructionMacro() { }

        /// <summary>
        /// Performs the action associated with the current instruction
        /// </summary>
        public void executeNextIntruction()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Advances the currentIntructin pointer to the next instruction, but does now actually execute the instruction
        /// </summary>
        public void advanceToNextInstruction()
        {
            throw new NotImplementedException();
        }

        
    }
}
