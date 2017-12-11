using System;
using System.Collections.Generic;
using Mogo.Game;

namespace Mogo.FSM
{
	public class StateHitDown : IState
	{
        public void Enter(EntityParent theOwner, params Object[] args)
        {
        }

        public void Exit(EntityParent theOwner, params Object[] args)
        {
        }

        public void Process(EntityParent theOwner, params Object[] args)
        {
            //不能受击
        }
	}
}
