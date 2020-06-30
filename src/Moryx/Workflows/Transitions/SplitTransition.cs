// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Workflows.Transitions
{
    /// <summary>
    /// Transition to split execution flow
    /// </summary>
    internal sealed class SplitTransition : TransitionBase
    {
        protected override void InputTokenAdded(object sender, IToken token)
        {
            Executing(delegate
            {
                ((IPlace)sender).Remove(token);
                // Split input into output
                foreach (var output in Outputs)
                {
                    output.Add(new SplitToken(token));
                }
            });
        }
    }
}