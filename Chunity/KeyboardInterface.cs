using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class KeyboardInterface : MonoBehaviour
{
    private static int revHold = 0;
    private static long[] reverse = { 0, 0, 0, 0 };
    private static int sweep = 0;
    private static int rand = 0;
    private static int stutter = 0;
    private static int stutDur = 2;
    private static int gate = 0;
    private static int gateDur = 3;
    private static int gateAdjust = 0;
    private static bool shiftPressed;
    private static long[] randomSelectVals = new long[16];
    private static long[] randomPositionVals = new long[16];
    private static int[] sampleDexes = {0,0,0,0};

    private readonly string[] samplePlayers = { "spatial_chuck", "spatial_chuck_2", "spatial_chuck_b", "spatial_chuck_b2" };

    private readonly KeyCode[] numbers = {KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
                                KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,
                                KeyCode.Alpha7, KeyCode.Alpha8};

    private readonly KeyCode[] letters = {KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,
                                KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
                                KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P};

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            // get shift key state
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                shiftPressed = true;
            else shiftPressed = false;

            // pressing escape reboots all main shreds
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "reboot", 1);
                }
            }

            // pressing [ selects the previous sequence in playlist
            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "prev", 1);
                }
            }

            // pressing ] selects the next sequence in playlist
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "next", 1);
                }
            }

            // pressing delete reverts to the last used sequence
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "back", 1);
                }
            }

            // pressing spacebar creates a randomized sequence
            if (Input.GetKeyDown(KeyCode.Space))
            {
                for (int i = 0; i < randomSelectVals.Length; i++)
                {
                    randomSelectVals[i] = Random.Range(0, 15);
                }
                for (int i = 0; i < randomPositionVals.Length; i++)
                {
                    randomPositionVals[i] = Random.Range(0, 7);
                }
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "rand", 1);
                    Chuck.Manager.SetIntArray(samplePlayers[i], "randomSelectVals", randomSelectVals);
                    Chuck.Manager.SetIntArray(samplePlayers[i], "randomPositionVals", randomPositionVals);
                }
            }

            // pressing tab swaps current sequence into playlist
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "swap", 1);
                }
            }

            // pressing numbers 1 - 8 jumps to that sequence in the playlist
            if (!shiftPressed)
            {
                for (int i = 0; i < numbers.Length; i++)
                {
                    if (Input.GetKeyDown(numbers[i]))
                    {
                        long[] jumpVals = { 1, i + 1 };
                        for (int j = 0; j < samplePlayers.Length; j++)
                        {
                            Chuck.Manager.SetIntArray(samplePlayers[j], "jump", jumpVals);
                        }
                        break;
                    }
                }
            }

            // pressing w toggles auto-hold for reverse playback
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (revHold == 1)
                {
                    revHold = 0;
                    for (int i = 0; i < reverse.Length; i++)
                    {
                        reverse[i] = 0;
                    }
                }
                else { revHold = 1; }
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "revHold", revHold);
                }
            }

            // pressing or holding r reverses playback for sample player 1
            if (revHold == 1)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    if (reverse[0] == 1) { reverse[0] = 0; }
                    else { reverse[0] = 1; }
                    Chuck.Manager.SetInt(samplePlayers[0], "reverse", reverse[0]);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.R))
                {
                    Chuck.Manager.SetInt(samplePlayers[0], "reverse", 1);
                }
            }

            // pressing or holding t reverses playback for sample player 2
            if (revHold == 1)
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    if (reverse[1] == 1) { reverse[1] = 0; }
                    else { reverse[1] = 1; }
                    Chuck.Manager.SetInt(samplePlayers[1], "reverse", reverse[1]);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.T))
                {
                    Chuck.Manager.SetInt(samplePlayers[1], "reverse", 1);
                }
            }

            // pressing or holding y reverses playback for sample player 3
            if (revHold == 1)
            {
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    if (reverse[2] == 1) { reverse[2] = 0; }
                    else { reverse[2] = 1; }
                    Chuck.Manager.SetInt(samplePlayers[2], "reverse", reverse[2]);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.Y))
                {
                    Chuck.Manager.SetInt(samplePlayers[2], "reverse", 1);
                }
            }

            // pressing or holding u reverses playback for sample player 3
            if (revHold == 1)
            {
                if (Input.GetKeyDown(KeyCode.U))
                {
                    if (reverse[3] == 1) { reverse[3] = 0; }
                    else { reverse[3] = 1; }
                    Chuck.Manager.SetInt(samplePlayers[3], "reverse", reverse[3]);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.U))
                {
                    Chuck.Manager.SetInt(samplePlayers[3], "reverse", 1);
                }
            }

            // pressing left alt toggles sequencer's step determination mode
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "stepMode", 1);
                }
            }

            // pressing z decreases playback speed (must reboot)
            if (Input.GetKeyDown(KeyCode.Z))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "downshift", 1);
                }
            }

            // pressing x increases playback speed (must reboot)
            if (Input.GetKeyDown(KeyCode.X))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "upshift", 1);
                }
            }

            // pressing s toggles filter sweeps in set direction
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (sweep == 1) { sweep = 0; }
                else { sweep = 1; }
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "sweep", sweep);
                }
            }

            // pressing ; sets sweep direction down
            if (Input.GetKeyDown(KeyCode.Semicolon))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "sweepDir", 0);
                }
            }

            // pressing ' sets sweep direction up
            if (Input.GetKeyDown(KeyCode.Quote))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "sweepDir", 1);
                }
            }

            // pressing or holding v performs tempo variance curves in set direction
            if (Input.GetKey(KeyCode.V))
            {
                Chuck.Manager.SetInt(samplePlayers[rand], "variance", 0);
                rand = Random.Range(0, 4); // but only on one random player
                Chuck.Manager.SetInt(samplePlayers[rand], "variance", 1);
            }

            // pressing , sets variance direction down
            if (Input.GetKeyDown(KeyCode.Comma))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "varianceDir", 0);
                }
            }

            // pressing . sets variance direction up
            if (Input.GetKeyDown(KeyCode.Period))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "varianceDir", 1);
                }
            }

            // pressing or holding / toggles a stutter of set duration
            if (Input.GetKey(KeyCode.Slash))
            {
                if (stutter == 1) { stutter = 0; }
                else { stutter = 1; }
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "stutter", stutter);
                }
            }

            // pressing 9 lengthens stutter duration
            if (!shiftPressed && Input.GetKeyDown(KeyCode.Alpha9))
            {
                if (stutDur > 1) { stutDur--; }
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "stutDur", stutDur);
                }
            }

            // pressing 0 shortens stutter duration
            if (!shiftPressed && Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (stutDur < 4) { stutDur++; }
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "stutDur", stutDur);
                }
            }

            // pressing \ toggles rhythmic gating of set duration
            if (Input.GetKeyDown(KeyCode.Backslash))
            {
                if (gate == 1) { gate = 0; }
                else { gate = 1; }
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "gate", gate);
                }
            }

            // pressing ( lengthens gate duration
            if (shiftPressed && Input.GetKeyDown(KeyCode.Alpha9))
            {
                if (gateDur > 1) { gateDur--; }
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "gateDur", gateDur);
                }
            }

            // pressing ) shortens gate duration
            if (shiftPressed && Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (gateDur < 4) { gateDur++; }
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "gateDur", gateDur);
                }
            }

            // pressing _ decreases audible period for gating
            if (shiftPressed && Input.GetKeyDown(KeyCode.Minus))
            {
                if (gateAdjust > -2) { gateAdjust--; }
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "gateAdjust", gateAdjust);
                }
            }

            // pressing + increases audible period for gating
            if (shiftPressed && Input.GetKeyDown(KeyCode.Equals))
            {
                if (gateAdjust < 2) { gateAdjust++; }
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "gateAdjust", gateAdjust);
                }
            }

            // pressing - writes last pressed select value in step
            if (!shiftPressed && Input.GetKeyDown(KeyCode.Minus))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "writeSelect", 1);
                }
            }

            // pressing = writes last pressed position value in step
            if (!shiftPressed && Input.GetKeyDown(KeyCode.Equals))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "writePosition", 1);
                }
            }

            // pressing letters a - p sets lastSelectVal up to write
            for (int i = 0; i < letters.Length; i++)
            {
                if (Input.GetKeyDown(letters[i]))
                {
                    for (int j = 0; j < samplePlayers.Length; j++)
                    {
                        Chuck.Manager.SetInt(samplePlayers[j], "lastSelectVal", i);
                    }
                    break;
                }
            }

            // pressing shift + numbers 1 - 8 sets lastPositionVal up to write
            if (shiftPressed)
            {
                for (int i = 0; i < numbers.Length; i++)
                {
                    if (Input.GetKeyDown(numbers[i]))
                    {
                        for (int j = 0; j < samplePlayers.Length; j++)
                        {
                            Chuck.Manager.SetInt(samplePlayers[j], "lastPositionVal", i);
                        }
                        break;
                    }
                }
            }

            // pressing left control/command + numbers 1-4 cycles through sample packs
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    sampleDexes[0]--;
                    if (sampleDexes[0] < 0) sampleDexes[0] = 4;
                    Chuck.Manager.SetInt(samplePlayers[0], "sampdex", sampleDexes[0]);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    sampleDexes[1]--;
                    if (sampleDexes[1] < 0) sampleDexes[1] = 4;
                    Chuck.Manager.SetInt(samplePlayers[1], "sampdex", sampleDexes[1]);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    sampleDexes[2]--;
                    if (sampleDexes[2] < 0) sampleDexes[2] = 4;
                    Chuck.Manager.SetInt(samplePlayers[2], "sampdex", sampleDexes[2]);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    sampleDexes[3]--;
                    if (sampleDexes[3] < 0) sampleDexes[3] = 4;
                    Chuck.Manager.SetInt(samplePlayers[3], "sampdex", sampleDexes[3]);
                }
            }
            else if (Input.GetKey(KeyCode.LeftCommand))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    sampleDexes[0]++;
                    if (sampleDexes[0] > 4) sampleDexes[0] = 0;
                    Chuck.Manager.SetInt(samplePlayers[0], "sampdex", sampleDexes[0]);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    sampleDexes[1]++;
                    if (sampleDexes[1] > 4) sampleDexes[1] = 0;
                    Chuck.Manager.SetInt(samplePlayers[1], "sampdex", sampleDexes[1]);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    sampleDexes[2]++;
                    if (sampleDexes[2] > 4) sampleDexes[2] = 0;
                    Chuck.Manager.SetInt(samplePlayers[2], "sampdex", sampleDexes[2]);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    sampleDexes[3]++;
                    if (sampleDexes[3] > 4) sampleDexes[3] = 0;
                    Chuck.Manager.SetInt(samplePlayers[3], "sampdex", sampleDexes[3]);
                }
            }

            // pressing shift + left control randomizes the sample packs
            if (shiftPressed)
            {
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    for (int i = 0; i < sampleDexes.Length; i++)
                    {
                        sampleDexes[i] = Random.Range(0, 5);
                        Chuck.Manager.SetInt(samplePlayers[i], "sampdex", sampleDexes[i]);
                    }
                }
            }

            // pressing q terminates all Chuck shreds
            if (Input.GetKey(KeyCode.Q))
            {
                for (int i = 0; i < samplePlayers.Length; i++)
                {
                    Chuck.Manager.SetInt(samplePlayers[i], "quit", 1);
                }
            }
        }
    }
}
