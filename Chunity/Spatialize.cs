using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Spatialize : MonoBehaviour {

    public AudioMixer mixerWithChuck;

    private string spatialChuck;
    private int revHold = 0;
    private long[] reverse = { 0, 0, 0, 0 };
    private int sweep = 0;
    private int stutter = 0;
    private int stutDur = 2;
    private int gate = 0;
    private int gateDur = 3;
    private int gateAdjust = 0;
    private bool shiftPressed;

    private KeyCode[] numbers = {KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
                                KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,
                                KeyCode.Alpha7, KeyCode.Alpha8};

    private KeyCode[] letters = {KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,
                                KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
                                KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P};


    // Use this for initialization
    void Start () {

        spatialChuck = "spatial_chuck";

        Chuck.Manager.Initialize( mixerWithChuck, spatialChuck);

        Chuck.Manager.RunCode(spatialChuck,
            @"

            0 => global int reboot;
            0 => global int prev;
            0 => global int next;
            0 => global int back;
            0 => global int rand;
            0 => global int swap;
            [0,0] @=> global int jump[];
            0 => global int revHold;
            [0,0,0,0] @=> global int reverse[];
            1 => global int stepMode;
            0 => global int downshift;
            0 => global int upshift;
            0 => global int sweep;
            0 => global int sweepDir;
            0 => global int variance;
            0 => global int varianceDir;
            0 => global int stutter;
            2 => global int stutDur;
            0 => global int gate;
            3 => global int gateDur;
            0 => global int gateAdjust;
            0 => global int writeSelect;
            0 => global int writePosition;
            global int lastSelectVal;
            global int lastPositionVal;

            class Keyboard {
                
                fun int getReboot ()
                {
                    if (reboot) {
                        0 => reboot;
                        return 1;
                    }
                    else { return 0; }
                }
                
                fun int getPrevious ()
                {
                    if (prev) {
                        0 => prev;
                        return 1;
                    }
                    else { return 0; }
                }
                
                fun int getNext ()
                {
                    if (next) {
                        0 => next;
                        return 1;
                    }
                    else { return 0; }
                }
                
                fun int getBack ()
                {
                    if (back) {
                        0 => back;
                        return 1;
                    }
                    else { return 0; }
                }
                
                fun int getRandom ()
                {
                    if (rand) {
                        0 => rand;
                        return 1;
                    }
                    else { return 0; }
                }
                
                fun int swapIn ()
                {
                    if (swap) {
                        0 => swap;
                        return 1;
                    }
                    else { return 0; }
                }
                
                fun int getJump ()
                {
                    if (jump[0]) {
                        0 => jump[0];
                        return jump[1];
                    }
                    else { return 0; }
                }
                
                fun int getReverse (int which)
                {
                    if (revHold) {
                        if (reverse[which]) { return 1; }
                        else { return 0; }
                    }
                    else {
                        if (reverse[which]) {
                            0 => reverse[which];
                            return 1;
                        }
                        else { return 0; }
                    }
                }
                
                fun int getStepMode () 
                {
                    if (stepMode) { 
                        0 => stepMode;
                        return 1; 
                    }
                    else { return 0; }
                }
                
                fun int getSpeed ()
                {
                    if (downshift) {
                        0 => downshift;
                        if (upshift) {
                            0 => upshift;
                            return 0;
                        }
                        return 1;
                    }
                    if (upshift) {
                        0 => upshift;
                        return 2;
                    }
                    else { return -1; }
                }
                
                fun int getSweep ()
                {
                    if (sweep) {
                        return sweepDir;
                    }
                    else { return -1; }
                }
                
                fun int getVariance ()
                {
                    if (variance) {
                        0 => variance;
                        return varianceDir;
                    }
                    else { return -1; }
                }
                
                fun int getStutter ()
                {
                    if (stutter) {
                        return stutDur;
                    }
                    else { return 0; }
                }
                
                fun int[] getGate ()
                {
                    if (gate) {
                        return [gateDur,gateAdjust];
                    }
                    else { return [0,0]; }
                }
                
                fun int getLastSelect ()
                {
                    if (writeSelect) {
                        0 => writeSelect;
                        return lastSelectVal;
                    }
                    else { return -1; }
                }
                
                fun int getLastPosition ()
                {
                    if (writePosition) {
                        0 => writePosition;
                        return lastPositionVal;
                    }
                    else { return -1; }
                }
                
            } // end Keyboard class

            class Sequencer {
                
                Keyboard kb;
                
                // select seed preset list (slot 0 stores last played sequence,*)
                [ [15,1,9,9,1,5,10,5],
                [7,3,14,2,10,6,5,11],
                [0,4,4,4,4,9,4,4],
                [0,4,4,4,4,9,4,4],
                [2,1,12,9,14,5,10,5],
                [11,14,13,3,1,14,14,7],
                [7,8,14,13,7,7,2,7],
                [3,14,8,0,4,10,4,1],
                [10,12,10,0,1,12,3,10],
                [12,8,1,11,13,3,9,5],
                [2,2,1,13,2,15,2,7] ] @=> int selectPack[][];
                
                // position seed preset list (*remaining slots are for playlist)
                [ [0,1,2,0,6,7,3,7],
                [0,4,5,3,5,7,5,1],
                [3,1,4,1,7,2,4,2],
                [3,1,4,1,5,9,2,6],
                [6,2,3,6,1,6,1,1],
                [1,5,3,5,0,7],
                [1,5,1,4,5,7],
                [2,2,5,4,1],
                [5,0,6,0,3],
                [3,1,2,5,5],
                [7,3,4,4,5] ] @=> int positionPack[][];
                
                // sequence playback buffers, holds the seeds currently being played
                selectPack[0] @=> int select[];
                positionPack[0] @=> int position[];
                // can be tweaked in real time using the step-insert keys ( - and = )
                
                0 => int opMode;
                0 => int seldex;
                0 => int posdex;
                [0,0,0,0] @=> int seldexPack[];
                [0,0,0,0] @=> int posdexPack[];
                1 => int scrolldex;
                1 => int lastScrolldex;
                0 => int navFlag;
                
                fun void scrollLeft()
                {
                    1 => navFlag;
                    scrolldex => lastScrolldex;
                    scrolldex--;
                    if (scrolldex < 1) { 1 => scrolldex; }
                    else {
                        select @=> selectPack[0];
                        position @=> positionPack[0];
                        selectPack[scrolldex] @=> select;
                        positionPack[scrolldex] @=> position;
                    }
                }
                
                fun void scrollRight()
                {
                    1 => navFlag;
                    scrolldex => lastScrolldex;
                    scrolldex++;
                    if (scrolldex > (selectPack.cap()-1)) {
                        (selectPack.cap()-1) => scrolldex;
                    }
                    else {
                        select @=> selectPack[0];
                        position @=> positionPack[0];
                        selectPack[scrolldex] @=> select;
                        positionPack[scrolldex] @=> position;
                    }
                }
                
                fun void goBack()
                {
                    if (navFlag) {
                        scrolldex => int tempdex;
                        lastScrolldex => scrolldex;
                        tempdex => lastScrolldex;
                    }
                    select @=> int seltemp[];
                    position @=> int postemp[];
                    selectPack[0] @=> select;
                    positionPack[0] @=> position;
                    seltemp @=> selectPack[0];
                    postemp @=> positionPack[0];
                }
                
                fun void randomize()
                {
                    0 => navFlag;
                    select @=> selectPack[0];
                    position @=> positionPack[0];
                    int seltemp[select.cap()];
                    int postemp[position.cap()];
                    for (0 => int i; i < seltemp.cap(); i++) {
                        Math.random2(0,15) => seltemp[i];
                    }
                    for (0 => int i; i < postemp.cap(); i++) {
                        Math.random2(0,7) => postemp[i];
                    }
                    seltemp @=> select;
                    postemp @=> position;
                }
                
                fun void swapInPlace()
                {
                    select @=> selectPack[scrolldex];
                    position @=> positionPack[scrolldex];
                }
                
                fun void jumpTo(int which)
                {
                    1 => navFlag;
                    scrolldex => lastScrolldex;
                    which => scrolldex;
                    if (scrolldex != lastScrolldex) {
                        select @=> selectPack[0];
                        position @=> positionPack[0];
                        selectPack[scrolldex] @=> select;
                        positionPack[scrolldex] @=> position;
                    }
                }
                
                fun int getNextSelect()
                {
                    kb.getLastSelect() => int lstemp;
                    seldex => int val;
                    seldex++;
                    if (seldex > (select.cap()-1)) { 0 => seldex; }
                    if (lstemp != -1) { lstemp => select[val]; }
                    return select[val];
                }
                
                fun int getNextSelect(int which)
                {
                    kb.getLastSelect() => int lstemp;
                    seldexPack[which] => int val;
                    seldexPack[which]++;
                    if (seldexPack[which] > (select.cap()-1)) { 0 => seldexPack[which]; }
                    if (lstemp != -1) { lstemp => select[val]; }
                    return select[val];
                }
                
                fun int getNextPosition()
                {
                    kb.getLastPosition() => int lptemp;
                    posdex => int val;
                    posdex++;
                    if (posdex > (position.cap()-1)) { 0 => posdex; }
                    if (lptemp != -1) { lptemp => position[val]; }
                    return position[val];
                }
                
                fun int getNextPosition(int which)
                {
                    kb.getLastPosition() => int lptemp;
                    posdexPack[which] => int val;
                    posdexPack[which]++;
                    if (posdexPack[which] > (position.cap()-1)) { 0 => posdexPack[which]; }
                    if (lptemp != -1) { lptemp => position[val]; }
                    return position[val];
                }
                
                fun void printSelectArray()
                {
                    chout <= ""| "";
                    for (0 => int i; i < select.cap(); i++) {
                        chout <= select[i] <= "" | "";
                    }
                    chout <= IO.newline();
                }
                
                fun void printPositionArray()
                {
                    chout <= ""| "";
                    for (0 => int i; i < position.cap(); i++) {
                        chout <= position[i] <= "" | "";
                    }
                    chout <= IO.newline();
                }
                
            }

            16 => int channels;
            SndBuf2 amen[channels];
            LPF lowpass[channels*2];
            HPF hipass[channels*2];
            Pan2 pan[channels];
            Gain leftChannels[channels];
            Gain rightChannels[channels];
            0 => int allsamples;
            Sequencer seq;

            for (0 => int i; i < channels; i++)
            {
                // spatializer input
                adc.chan(0) => leftChannels[i] => dac.chan(0);
                adc.chan(1) => rightChannels[i] => dac.chan(1);

                // sample input
                amen[i].chan(0) => lowpass[i] => hipass[i] => pan[i].chan(0) => leftChannels[i];
                amen[i].chan(1) => lowpass[i+channels] => hipass[i+channels] => pan[i].chan(1) => rightChannels[i];

                // BYOL(bring your own loops), wants 16 of them labeled 0.wav - 15.wav
                me.dir() + ""/amen/loops/"" + i + "".wav"" => amen[i].read;
                // all samples must have the same length and tempo

                amen[i].samples() +=> allsamples;
                amen[i].samples() => amen[i].pos;

                // multiply sample output with spatializer input
                3 => leftChannels[i].op;
                3 => rightChannels[i].op;

                20000.0 => lowpass[i].freq;
                20000.0 => lowpass[i+channels].freq;
                20.0 => hipass[i].freq;
                20.0 => hipass[i+channels].freq;
                1.9 => lowpass[i].Q;
                2.2 => lowpass[i+channels].Q;
                1.9 => hipass[i].Q;
                2.2 => hipass[i+channels].Q;
            }

            16 /=> allsamples;
            allsamples/2 => int bar;
            bar/4 => int beat;

            Shred mainShreds[10];
            0 => int shredCount;
            0 => int flush;

            // modifications to rate require inverse ratio as multiplier to tempo
            [0.8,1.0,1.25] @=> float rate[];
            [1.25,1.0,0.8] @=> float tempoMod[];
            0 => int ratedex;

            [0,0,0,0] @=> int selectVals[];
            [0.0,0.0,0.0,0.0] @=> float gainVals[];

            fun void playAmen(int id, float gain, float rate, float tempo)
            {
                gain => gainVals[id];
                while (true)
                {
                    int selector;
                    int position;
                    if (seq.opMode) {
                        seq.getNextSelect(id) => selector;
                        beat*(seq.getNextPosition(id)) => position;
                    }
                    else {
                        seq.getNextSelect() => selector;
                        beat*(seq.getNextPosition()) => position;
                    }
                    selector => selectVals[id];
                    position => amen[selector].pos;
                    gain => amen[selector].gain;
                    if (seq.kb.getReverse(id)) { -rate => amen[selector].rate; }
                    else { rate => amen[selector].rate; }
                    Math.random2f(-0.05,0.05) => pan[selector].pan;
                    
                    seq.kb.getSweep() => int sweepDir;
                    seq.kb.getVariance() => int varDir;
                    if ((varDir >= 0) || (sweepDir >= 0)) {
                        seq.kb.getStutter() => int result;
                        if (result > 0) {
                            now + tempo::samp => time later;
                            tempo / (Math.pow(2,result)) => float duration;
                            while (now < later) {
                                position => amen[selector].pos;
                                duration::samp => now;
                            }
                            0 => stutter;
                        }
                        else {
                            now + (tempo/2)::samp => time later1;
                            now + tempo::samp => time later2;
                            rate => float rateVariance;
                            -0.00005 => float varInc;
                            if (varDir) { 0.00005 => varInc; }
                            varInc * (id + 1) => varInc;
                            20000.0 => float lpFreq;
                            20.0 => float hpFreq;
                            0.33 * (Math.pow(2,id)) => float lpInc;
                            0.005 * (Math.pow(2,id)) => float hpInc;
                            if (varDir >= 0) {
                                if (sweepDir >= 0) {
                                    if (sweepDir) {
                                        while (now < later2) {
                                            0.00005 +=> hpInc;
                                            hpInc +=> hpFreq;
                                            if (hpFreq < 20000.0) {
                                                hpFreq => hipass[selector].freq;
                                                hpFreq => hipass[selector+channels].freq;
                                            }
                                            if (now < later1) {
                                                varInc +=> rateVariance;
                                            }
                                            else { varInc -=> rateVariance; }
                                            rateVariance => amen[selector].rate;
                                            1::samp => now;
                                        }
                                    }
                                    else {
                                        while (now < later2) {
                                            0.000005 -=> lpInc;
                                            lpInc -=> lpFreq;
                                            if (lpFreq > 20.0) {
                                                lpFreq => lowpass[selector].freq;
                                                lpFreq => lowpass[selector+channels].freq;
                                            }
                                            if (now < later1) {
                                                varInc +=> rateVariance;
                                            }
                                            else { varInc -=> rateVariance; }
                                            rateVariance => amen[selector].rate;
                                            1::samp => now;
                                        }
                                    }
                                }
                                else {
                                    while (now < later2) {
                                        if (now < later1) { varInc +=> rateVariance; }
                                        else { varInc -=> rateVariance; }
                                        rateVariance => amen[selector].rate;
                                        1::samp => now;
                                    }
                                }
                            }
                            if (sweepDir >= 0) {
                                while (now < later2) {
                                    if (sweepDir) {
                                        0.00005 +=> hpInc;
                                        hpInc +=> hpFreq;
                                        if (hpFreq < 20000.0) {
                                            hpFreq => hipass[selector].freq;
                                            hpFreq => hipass[selector+channels].freq;
                                        }
                                    }
                                    else {
                                        0.000005 -=> lpInc;
                                        lpInc -=> lpFreq;
                                        if (lpFreq > 20.0) {
                                            lpFreq => lowpass[selector].freq;
                                            lpFreq => lowpass[selector+channels].freq;
                                        }
                                    }
                                    1::samp => now;
                                }
                            }
                        }
                    }
                    else {
                        seq.kb.getStutter() => int result;
                        if (result > 0) {
                            now + tempo::samp => time later;
                            tempo / (Math.pow(2,result)) => float duration;
                            while (now < later) {
                                position => amen[selector].pos;
                                duration::samp => now;
                            }
                            0 => stutter;
                        }
                        else { tempo::samp => now; }
                    }
                    
                    20000.0 => lowpass[selector].freq;
                    20000.0 => lowpass[selector+channels].freq;
                    20.0 => hipass[selector].freq;
                    20.0 => hipass[selector+channels].freq;
                    amen[selector].samples() => amen[selector].pos;
                    
                    if (seq.kb.getReboot() || flush) {
                        1 => flush;
                        shredCount--;
                        me.exit();
                    }
                }
            }

            fun void printArrays(Sequencer s)
            {
                while (true) {
                    s.printSelectArray();
                    s.printPositionArray();
                    chout <= IO.newline();
                    0.25::second => now; // can refine display refresh period to taste
                    
                    if (seq.kb.getReboot() || flush) {
                        1 => flush;
                        shredCount--;
                        me.exit();
                    }
                }
            }

            fun void updatePlaylist()
            {
                while (true) {
                    (tempoMod[ratedex] * (beat/2))::samp => now;
                    if (seq.kb.getPrevious()) { seq.scrollLeft(); }
                    if (seq.kb.getNext()) { seq.scrollRight(); }
                    if (seq.kb.getBack()) { seq.goBack(); }
                    if (seq.kb.swapIn()) { seq.swapInPlace(); }
                    seq.kb.getJump() => int result;
                    if (result > 0) { seq.jumpTo(result); }
                    
                    if (seq.kb.getReboot() || flush) {
                        1 => flush;
                        shredCount--;
                        me.exit();
                    }
                }
            }

            fun void mixItUp()
            {
                while (true) {
                    (tempoMod[ratedex] * (beat/2))::samp => now;
                    if (seq.kb.getRandom()) { seq.randomize(); }
                    
                    if (seq.kb.getReboot() || flush) {
                        1 => flush;
                        shredCount--;
                        me.exit();
                    }
                }
            }

            fun void speedListener() {
                while( true ) {
                    (tempoMod[ratedex] * (beat/2))::samp => now;
                    seq.kb.getSpeed() => int result;
                    if (result > 0) {
                        if (result == 1) {
                            ratedex--;
                            if (ratedex < 0) { 0 => ratedex; }
                        }
                        if (result == 2) {
                            ratedex++;
                            if (ratedex > 2) { 2 => ratedex; }
                        }
                    }
                    
                    if (seq.kb.getReboot() || flush) {
                        1 => flush;
                        shredCount--;
                        me.exit();
                    }
                }
            }

            fun void stepModeListener() {
                while( true ) {
                    (tempoMod[ratedex] * (beat/2))::samp => now;
                    if (seq.kb.getStepMode()) {
                        if (seq.opMode) { 0 => seq.opMode; }
                        else { 1 => seq.opMode; }
                    }
                    
                    if (seq.kb.getReboot() || flush) {
                        1 => flush;
                        shredCount--;
                        me.exit();
                    }
                }
            }

            fun void gateListener() {
                while( true ) {
                    seq.kb.getGate() @=> int result[];
                    if (result[0] > 0) {
                        Math.pow(2,result[0]) => float factor;
                        tempoMod[ratedex] * (beat/factor) => float duration;
                        (duration/4) * result[1] => float adjustment;
                        1 => int gateSwitch;
                        for (0 => int i; i < factor; i++) {
                            if (gateSwitch) {
                                for (0 => int j; j < selectVals.cap(); j++) {
                                    gainVals[j] => amen[selectVals[j]].gain;
                                }
                                (duration + adjustment)::samp => now;
                                0 => gateSwitch;
                            }
                            else {
                                for (0 => int j; j < selectVals.cap(); j++) {
                                    0.0 => amen[selectVals[j]].gain;
                                }
                                (duration - adjustment)::samp => now;
                                1 => gateSwitch;
                            }
                        }
                    }
                    else { (tempoMod[ratedex] * (beat/2))::samp => now; }
                    
                    if (seq.kb.getReboot() || flush) {
                        1 => flush;
                        shredCount--;
                        me.exit();
                    }
                }
            }

            fun void rollOut()
            {
                spork ~ playAmen(0, 1.0, rate[ratedex], (tempoMod[ratedex]*bar)) @=> mainShreds[0];
                spork ~ playAmen(1, 0.9, rate[ratedex], (tempoMod[ratedex]*(bar/2))) @=> mainShreds[1];
                spork ~ playAmen(2, 0.75, rate[ratedex], (tempoMod[ratedex]*beat)) @=> mainShreds[2];
                spork ~ playAmen(3, 0.5, rate[ratedex], (tempoMod[ratedex]*(beat/2))) @=> mainShreds[3];
                
                spork ~ printArrays(seq) @=> mainShreds[4];
                spork ~ updatePlaylist() @=> mainShreds[5];
                spork ~ mixItUp() @=> mainShreds[6];
                spork ~ speedListener() @=> mainShreds[7];
                spork ~ stepModeListener() @=> mainShreds[8];
                spork ~ gateListener() @=> mainShreds[9];
                
                10 => shredCount;
                0 => flush;
            }

            rollOut();

            fun void resetListener() {
                while( true ) {
                    (tempoMod[ratedex] * (beat/2))::samp => now;
                    if (shredCount < 10) {
                        mainShreds[0].exit();
                        mainShreds[1].exit();
                        mainShreds[2].exit();
                        mainShreds[3].exit();
                        mainShreds[4].exit();
                        mainShreds[5].exit();
                        mainShreds[6].exit();
                        mainShreds[7].exit();
                        mainShreds[8].exit();
                        mainShreds[9].exit();
                        rollOut();
                    }
                }
            }

            spork ~ resetListener();

            while( true ) // keep the fire alive
                1::second => now;
            "
        );
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.anyKey)
        {
            // get shift key state
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                shiftPressed = true;
            else shiftPressed = false;

            // pressing escape reboots all main shreds
            if (Input.GetKeyDown(KeyCode.Escape))
                Chuck.Manager.SetInt(spatialChuck, "reboot", 1);

            // pressing [ selects the previous sequence in playlist
            if (Input.GetKeyDown(KeyCode.LeftBracket))
                Chuck.Manager.SetInt(spatialChuck, "prev", 1);

            // pressing ] selects the next sequence in playlist
            if (Input.GetKeyDown(KeyCode.RightBracket))
                Chuck.Manager.SetInt(spatialChuck, "next", 1);

            // pressing delete reverts to the last used sequence
            if (Input.GetKeyDown(KeyCode.Backspace))
                Chuck.Manager.SetInt(spatialChuck, "back", 1);

            // pressing spacebar creates a randomized sequence
            if (Input.GetKeyDown(KeyCode.Space))
                Chuck.Manager.SetInt(spatialChuck, "rand", 1);

            // pressing tab swaps current sequence into playlist
            if (Input.GetKeyDown(KeyCode.Tab))
                Chuck.Manager.SetInt(spatialChuck, "swap", 1);

            // pressing numbers 1 - 8 jumps to that sequence in the playlist
            if (!shiftPressed) {
                for (int i = 0; i < numbers.Length; i++) {
                    if (Input.GetKeyDown(numbers[i])) {
                        long[] jumpVals = { 1, i + 1 };
                        Chuck.Manager.SetIntArray(spatialChuck, "jump", jumpVals);
                        break;
                    }
                }
            }

            // pressing w toggles auto-hold for reverse playback
            if (Input.GetKeyDown(KeyCode.W)) {
                if (revHold == 1) { revHold = 0; }
                else { revHold = 1; }
                Chuck.Manager.SetInt(spatialChuck, "revHold", revHold);
            }

            // pressing/holding r reverses playback for sample player 1
            if (Input.GetKey(KeyCode.R)) {
                if (revHold == 1) {
                    if (reverse[0] == 1) { reverse[0] = 0; }
                    else { reverse[0] = 1; }
                }
                else { reverse[0] = 1; }
                Chuck.Manager.SetIntArray(spatialChuck, "reverse", reverse);
            }

            // pressing/holding t reverses playback for sample player 2
            if (Input.GetKey(KeyCode.T)) {
                if (revHold == 1) {
                    if (reverse[1] == 1) { reverse[1] = 0; }
                    else { reverse[1] = 1; }
                }
                else { reverse[1] = 1; }
                Chuck.Manager.SetIntArray(spatialChuck, "reverse", reverse);
            }

            // pressing/holding y reverses playback for sample player 3
            if (Input.GetKey(KeyCode.Y)) {
                if (revHold == 1) {
                    if (reverse[2] == 1) { reverse[2] = 0; }
                    else { reverse[2] = 1; }
                }
                else { reverse[2] = 1; }
                Chuck.Manager.SetIntArray(spatialChuck, "reverse", reverse);
            }

            // pressing/holding u reverses playback for sample player 4
            if (Input.GetKey(KeyCode.U)) {
                if (revHold == 1) {
                    if (reverse[3] == 1) { reverse[3] = 0; }
                    else { reverse[3] = 1; }
                }
                else { reverse[3] = 1; }
                Chuck.Manager.SetIntArray(spatialChuck, "reverse", reverse);
            }

            // pressing q toggles sequencer's step determination mode
            if (Input.GetKeyDown(KeyCode.Q))
                Chuck.Manager.SetInt(spatialChuck, "stepMode", 1);

            // pressing z decreases playback speed (must reboot)
            if (Input.GetKeyDown(KeyCode.Z))
                Chuck.Manager.SetInt(spatialChuck, "downshift", 1);

            // pressing x increases playback speed (must reboot)
            if (Input.GetKeyDown(KeyCode.X))
                Chuck.Manager.SetInt(spatialChuck, "upshift", 1);

            // pressing s toggles filter sweeps in set direction
            if (Input.GetKeyDown(KeyCode.S)) {
                if (sweep == 1) { sweep = 0; }
                else { sweep = 1; }
                Chuck.Manager.SetInt(spatialChuck, "sweep", sweep);
            }

            // pressing ; sets sweep direction down
            if (Input.GetKeyDown(KeyCode.Semicolon))
                Chuck.Manager.SetInt(spatialChuck, "sweepDir", 0);

            // pressing ' sets sweep direction up
            if (Input.GetKeyDown(KeyCode.Quote))
                Chuck.Manager.SetInt(spatialChuck, "sweepDir", 1);

            // holding v performs tempo variance curves in set direction
            if (Input.GetKey(KeyCode.V))
                Chuck.Manager.SetInt(spatialChuck, "variance", 1);

            // pressing , sets variance direction down
            if (Input.GetKeyDown(KeyCode.Comma))
                Chuck.Manager.SetInt(spatialChuck, "varianceDir", 0);

            // pressing . sets variance direction up
            if (Input.GetKeyDown(KeyCode.Period))
                Chuck.Manager.SetInt(spatialChuck, "varianceDir", 1);

            // pressing / toggles a stutter of set duration
            if (Input.GetKey(KeyCode.Slash)) {
                if (stutter == 1) { stutter = 0; }
                else { stutter = 1; }
                Chuck.Manager.SetInt(spatialChuck, "stutter", stutter);
            }

            // pressing 9 lengthens stutter duration
            if (!shiftPressed && Input.GetKeyDown(KeyCode.Alpha9)) {
                if (stutDur > 1) { stutDur--; }
                Chuck.Manager.SetInt(spatialChuck, "stutDur", stutDur);
            }

            // pressing 0 shortens stutter duration
            if (!shiftPressed && Input.GetKeyDown(KeyCode.Alpha0)) {
                if (stutDur < 4) { stutDur++; }
                Chuck.Manager.SetInt(spatialChuck, "stutDur", stutDur);
            }

            // pressing \ toggles rhythmic gating of set duration
            if (Input.GetKeyDown(KeyCode.Backslash)) {
                if (gate == 1) { gate = 0; }
                else { gate = 1; }
                Chuck.Manager.SetInt(spatialChuck, "gate", gate);
            }

            // pressing ( lengthens gate duration
            if (shiftPressed && Input.GetKeyDown(KeyCode.Alpha9)) {
                if (gateDur > 1) { gateDur--; }
                Chuck.Manager.SetInt(spatialChuck, "gateDur", gateDur);
            }

            // pressing ) shortens gate duration
            if (shiftPressed && Input.GetKeyDown(KeyCode.Alpha0)) {
                if (gateDur < 4) { gateDur++; }
                Chuck.Manager.SetInt(spatialChuck, "gateDur", gateDur);
            }

            // pressing _ decreases audible period for gating
            if (shiftPressed && Input.GetKeyDown(KeyCode.Minus)) {
                if (gateAdjust > -2) { gateAdjust--; }
                Chuck.Manager.SetInt(spatialChuck, "gateAdjust", gateAdjust);
            }

            // pressing + increases audible period for gating
            if (shiftPressed && Input.GetKeyDown(KeyCode.Equals)) {
                if (gateAdjust < 2) { gateAdjust++; }
                Chuck.Manager.SetInt(spatialChuck, "gateAdjust", gateAdjust);
            }

            // pressing - writes last pressed select value in step
            if (!shiftPressed && Input.GetKeyDown(KeyCode.Minus))
                Chuck.Manager.SetInt(spatialChuck, "writeSelect", 1);

            // pressing = writes last pressed position value in step
            if (!shiftPressed && Input.GetKeyDown(KeyCode.Equals))
                Chuck.Manager.SetInt(spatialChuck, "writePosition", 1);

            // pressing letters a - p sets lastSelectVal up to write
            for (int i = 0; i < letters.Length; i++) {
                if (Input.GetKeyDown(letters[i])) {
                    Chuck.Manager.SetInt(spatialChuck, "lastSelectVal", i);
                    break;
                }
            }

            // pressing shift + numbers 1 - 8 sets lastPositionVal up to write
            if (shiftPressed) {
                for (int i = 0; i < numbers.Length; i++) {
                    if (Input.GetKeyDown(numbers[i])) {
                        Chuck.Manager.SetInt(spatialChuck, "lastPositionVal", i);
                        break;
                    }
                }
            }
        }

    }

}
