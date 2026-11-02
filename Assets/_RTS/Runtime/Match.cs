using System;
using System.Collections.Generic;
using UnityEngine;
namespace RTS {
public enum Phase { Preparation, Combat, Victory, Defeat, Draw }
public enum Mode { Attack, Defend, Aggressive }
[Serializable] public class Group { public int type, slot; public bool placed; public Vector2 position; public Mode mode; }
[Serializable] public class Soldier { public int faction,type,group; public Vector2 position,destination; public float health=100, cooldown; }
[Serializable] public class Match {
 public Phase phase; public float remaining=30; public int coins=1000; public List<Group> groups=new(); public List<Soldier> soldiers=new();
 public static readonly string[] Names={"Infantry","Spearman","Archer","Cavalry","Rifleman","Machine Gun","Cannon","Medic"};
 public static readonly int[] Counts={30,25,20,12,18,6,4,8}, Prices={100,100,100,150,100,125,150,100};
 public bool Buy(int type) { if(phase!=Phase.Preparation||groups.Count>=10||type<0||type>=8||coins<Prices[type])return false; int slot=0;while(groups.Exists(g=>g.slot==slot))slot++; groups.Add(new Group{type=type,slot=slot});coins-=Prices[type];return true; }
 public bool Place(int slot,Vector2 p) { var g=groups.Find(x=>x.slot==slot);if(phase!=Phase.Preparation||g==null||p.x<3||p.x>61||p.y<3||p.y>29||groups.Exists(x=>x!=g&&x.placed&&Vector2.Distance(x.position,p)<6))return false;g.position=p;g.placed=true;return true; }
 void Spawn(int faction,int type,int group,Vector2 p){for(int i=0;i<Counts[type];i++){var q=p+new Vector2(i%6*.5f,i/6*.5f);soldiers.Add(new Soldier{faction=faction,type=type,group=group,position=q,destination=q});}}
 public void Step(float dt){if(phase==Phase.Preparation){remaining=Mathf.Max(0,remaining-dt);if(remaining>0)return;foreach(var g in groups)if(!g.placed)coins+=Prices[g.type];groups.RemoveAll(g=>!g.placed);foreach(var g in groups)Spawn(0,g.type,g.slot,g.position);for(int t=0;t<8;t++)Spawn(1,t,t,new Vector2(100+t%2*10,3+t/2*7));phase=Phase.Combat;}
 if(phase!=Phase.Combat)return;
 foreach(var s in soldiers){if(s.health<=0)continue;s.cooldown-=dt;Soldier target=null;float best=float.MaxValue;foreach(var other in soldiers){if(other.faction==s.faction||other.health<=0)continue;float d=(other.position-s.position).sqrMagnitude;if(d<best){best=d;target=other;}}if(target==null)continue;float range=s.type==2||s.type>=4&&s.type<=6?8:1;
 if(best<144){if(best<=range*range){if(s.cooldown<=0){target.health-=s.type==6?35:10;s.cooldown=s.type==5?.2f:1;}}else s.destination=target.position;}
 else if(s.faction==1)s.destination=new Vector2(25,16);
 if(best>range*range)s.position=Vector2.MoveTowards(s.position,s.destination,dt*(s.type==3?5:2.5f));}
 bool a=soldiers.Exists(s=>s.faction==0&&s.health>0),b=soldiers.Exists(s=>s.faction==1&&s.health>0);if(!a||!b)phase=!a&&!b?Phase.Draw:a?Phase.Victory:Phase.Defeat;
 }
}
}
