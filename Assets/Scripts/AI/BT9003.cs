namespace Mogo.AI.BT
{
	public sealed class BT9003 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT9003 _instance = null;
		public static BT9003 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT9003();

				return _instance;
			}
		}

		private BT9003()
		{
			{
				Mogo.AI.SelectorNode node1 = new Mogo.AI.SelectorNode();
				this.AddChild(node1);
				{
					Mogo.AI.SequenceNode node2 = new Mogo.AI.SequenceNode();
					node1.AddChild(node2);
					node2.AddChild(new Mogo.AI.InSkillCoolDown(2));
					node2.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
					node2.AddChild(new Mogo.AI.CastSpell(2,0));
				}
				{
					Mogo.AI.SequenceNode node6 = new Mogo.AI.SequenceNode();
					node1.AddChild(node6);
					node6.AddChild(new Mogo.AI.InSkillCoolDown(1));
					node6.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
					node6.AddChild(new Mogo.AI.CastSpell(1,0));
				}
			}
		}
	}
}
