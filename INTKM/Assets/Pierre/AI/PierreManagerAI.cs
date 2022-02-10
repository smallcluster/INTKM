using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;



public class PierreManagerAI : MonoBehaviour
{

    // [actor: numPredators]
    private Dictionary<PierreActor, int> entities = new Dictionary<PierreActor, int>();
    private int numMaxPredators = 2;

    private int nbDogsAlive = 0;
    private int nbCatsAlive = 0;
    private int nbLizardsAlive = 0;


    private int nbPlayersAlive = 2;


    public List<PierrePlayer> players;

    public void AddActor(PierreActor e)
    {
        // No predators at first
        entities.Add(e, 0);
    }

    public void EnableAll()
    {
        foreach (KeyValuePair<PierreActor, int> e in entities)
        {
            e.Key.activated = true;
        }
    }
    public void DesableAll()
    {
        foreach (KeyValuePair<PierreActor, int> e in entities)
        {
            e.Key.activated = false;
        }
    }


    public bool allLizardAreDead()
    {
        return nbLizardsAlive <= 0;
    }

    public void IncreaseDogsNumber() { nbDogsAlive++; }
    public void IncreaseCatsNumber() { nbCatsAlive++; }
    public void IncreaseLizardsNumber() { nbLizardsAlive++; }
    public void DecreaseDogsNumber() { nbDogsAlive--; }
    public void DecreaseCatsNumber() { nbCatsAlive--; }
    public void DecreaseLizardsNumber() { nbLizardsAlive--; }

    public List<PierreActor> GetEnemies(string factions)
    {
        List<PierreActor> e = new List<PierreActor>();
        foreach (KeyValuePair<PierreActor, int> ai in entities)
        {
            if (!PierreFactionType.AreAllies(factions, ai.Key.GetFactions()))
                e.Add(ai.Key);
        }
        return e;
    }

    public PierreActor FindTarget(PierreActor source)
    {

        // The source no longer seek to attack it's target
        Untarget(source.target);
        

        // Find valid enemies
        List<PierreActor> validEnemies = new List<PierreActor>();
        foreach(KeyValuePair<PierreActor, int> e in entities)
        {
            if (!PierreFactionType.AreAllies(source.GetFactions(), e.Key.GetFactions()) && e.Value < numMaxPredators && !e.Key.IsDead() && e.Key.activated)
                validEnemies.Add(e.Key);
        }

        // Need at least one enemy
        if (validEnemies.Count == 0) return null;

        PierreActor target = validEnemies[0];

        // Find the closest enemy
        float dd1 = (target.transform.position - source.transform.position).sqrMagnitude;
        foreach (PierreActor e in validEnemies)
        {
            float dd2 = (e.transform.position - source.transform.position).sqrMagnitude;
            if (dd2 < dd1)
            {
                dd1 = dd2;
                target = e;
            }
        }

        // Target is being targeted by the source
        entities[target]++;

        return target;
    }

    // The Actor is no longer targeted by an other entity
    public void Untarget(PierreActor target)
    {
        if(target != null && entities[target] > 0)
            entities[target]--;
    }


    public List<PierreActor> GetAllActors()
    {
        return entities.Keys.ToList<PierreActor>();
    }

}
