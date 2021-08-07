# unity-tbrpg-realtime-arena
Realtime multiplayer game mode for Turnbase RPG template

## Development plan
- Uses [Colyseus](https://www.colyseus.io/) for room creation, match making, and turnbase battle
- For the early access version, it will not have match making yet (no button which press then random player to battle) but it will have room creation, room list and joining.
- Add room list, room creation and joining UIs
- Implement realtime battle
- Develop match making system after realtime battle is implemented
- Add match making UIs

## Workflow
- Players start match making with ID and token as a payload
- Server read the payload then validate user and bring formation data from game service (so the game service must have functions to get formation and other data)
- Server create a room after players matched, then send room ID to clients
- Clients join the room by recieved room ID
- Clients tells server that the battle scene is loaded and ready to play
- Game start, the first turn is for the client who own character which has highest speed stats
- Player has a few time to decision to attack or use skills
- When player decided, server will simulate character action and broadcast result to all clients
- Game will ends when any side leave the game or all characters are dead
- When wlend game, server may tell game-service to record it for rewarding later

## Note
- Must add some rule such as: each player have limited duration to make decision
- Have to think about whats to do when player exit or disconnect during the battle
