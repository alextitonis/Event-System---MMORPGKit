using LiteNetLibManager;
using MultiplayerARPG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class IEvent : LiteNetLibBehaviour
{
    [SerializeField] EventSettings settings;
    [SerializeField] StateTime[] states;

    public EventState currentState { get; protected set; }
    LiteNetLibSyncList<Team> teams = new LiteNetLibSyncList<Team>();
    LiteNetLibSyncList<EventPlayer> players = new LiteNetLibSyncList<EventPlayer>();

    void Start()
    {
        if (settings == null)
        {
            Debug.Log("Settings for event not found!");
            EventManager.getInstance.EventFinished(this);
            Destroy(gameObject);
        }

        CreateTeams();

        currentState = EventState.None;
        Invoke("NextState", getStateTime(0));
    }

    void CreateTeams()
    {
        if (settings.isGlobal)
        {
            TeamInfo t = null;
            if (settings.teams.Length == 1)
                t = settings.teams[0];
            else
                t = new TeamInfo { ID = 0, color = Color.blue, name = "Global" };

            teams.Add(new Team(t));
        }
        else
        {
            for (int i = 0; i < settings.teamCount; i++)
                teams.Add(new Team(settings.teams[i]));
        }
    }

    public void NextState()
    {
        currentState++;

        if (currentState == EventState.Finished)
        {
            EventManager.getInstance.EventFinished(this);
            return;
        }

        Invoke("NextState", getStateTime(currentState + 1));
    }

    public float getStateTime(EventState state)
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (states[i].state == state)
                return states[i].time;
        }

        return -1;
    }

    void SetPlayersToTeams()
    {
        if (settings.isGlobal)
            return;

        List<EventPlayer> players = new List<EventPlayer>();
        foreach (var player in this.players)
            players.Add(player);
        players.Shuffle();

        switch (settings.teamCount)
        {
            case 2:
                {
                    Team t1 = new Team(settings.teams[0]);
                    Team t2 = new Team(settings.teams[1]);
                    int lastTeam = 0;

                    foreach (var player in players)
                    {
                        lastTeam++;
                        if (lastTeam > 2)
                            lastTeam = 1;

                        if (lastTeam == 1)
                            t1.players.Add(player);
                        else if (lastTeam == 2)
                            t2.players.Add(player);
                    }

                    teams.Add(t1);
                    teams.Add(t2);
                }
                break;
            case 3:
                {
                    Team t1 = new Team(settings.teams[0]);
                    Team t2 = new Team(settings.teams[1]);
                    Team t3 = new Team(settings.teams[2]);
                    int lastTeam = 0;

                    foreach (var player in players)
                    {
                        lastTeam++;
                        if (lastTeam > 3)
                            lastTeam = 1;

                        if (lastTeam == 1)
                            t1.players.Add(player);
                        else if (lastTeam == 2)
                            t2.players.Add(player);
                        else if (lastTeam == 3)
                            t3.players.Add(player);
                    }

                    teams.Add(t1);
                    teams.Add(t2);
                    teams.Add(t3);
                }
                break;
            case 4:
                {
                    Team t1 = new Team(settings.teams[0]);
                    Team t2 = new Team(settings.teams[1]);
                    Team t3 = new Team(settings.teams[2]);
                    Team t4 = new Team(settings.teams[3]);
                    int lastTeam = 0;

                    foreach (var player in players)
                    {
                        lastTeam++;
                        if (lastTeam > 4)
                            lastTeam = 1;

                        if (lastTeam == 1)
                            t1.players.Add(player);
                        else if (lastTeam == 2)
                            t2.players.Add(player);
                        else if (lastTeam == 3)
                            t3.players.Add(player);
                        else if (lastTeam == 4)
                            t4.players.Add(player);
                    }

                    teams.Add(t1);
                    teams.Add(t2);
                    teams.Add(t3);
                    teams.Add(t4);
                }
                break;
        }
    }

    public void Register(PlayerCharacterEntity e)
    {
        if (e == null)
            return;

        if (teams.Count == 0)
            return;

        EventPlayer p = new EventPlayer { player = e, lastPosition = e.transform.position };

        if (settings.isGlobal)
        {
            p.team = teams[0].info;
            teams[0].players.Add(p);
        }
        else
            players.Add(p);
    }
    public void UnRegister(PlayerCharacterEntity e)
    {
        if (e == null)
            return;

        for (int i = 0; i < teams.Count; i++)
        {
            for (int j = 0; j < teams[i].players.Count; j++)
            {
                if (teams[i].players[j].player == e)
                {
                    teams[i].players.RemoveAt(j);
                    return;
                }
            }
        }
    }

    EventPlayer addPlayerToTeam(EventPlayer p)
    {
        return p;
    }
}

[System.Serializable]
public class EventSettings
{
    public string name;
    public bool isGlobal;
    public bool teleportOnStart;
    public bool teleportBackOnFinish;
    public int maxPlayers;
    public TeamInfo[] teams;

    public bool isTeamGame { get { return teams.Length > 0; } }
    public int teamCount { get { return teams.Length; } }
}

[System.Serializable]
public class TeamInfo
{
    public int ID;
    public string name;
    public Color color;
}

public class Team
{
    public TeamInfo info;
    public List<EventPlayer> players;

    public Team(TeamInfo info)
    {
        this.info = info;

        players = new List<EventPlayer>();
    }
}

[System.Serializable]
public class EventPlayer
{
    public PlayerCharacterEntity player;
    public Vector3 lastPosition;
    public TeamInfo team;
}

[System.Serializable]
public class StateTime
{
    public EventState state;
    public float time;
}

public enum EventState
{
    None = 0,
    Announcement = 1,
    Joining = 2,
    Started = 3,
    Finishing = 4,
    GaveRewards = 5,
    Finished = 6,
}