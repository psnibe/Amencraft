﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SpatializedSamplePlayer : MonoBehaviour
{
    public AudioMixer mixerWithChuck;
    public int samplePlayerID;

    Chuck.IntCallback selectCallback;
    Chuck.VoidCallback syncSelectCallback;

    private int selectValue;
    private float xPos;
    private float yPos;
    private float zPos;
    private Vector3 newPosition;
    private bool synced;

    private readonly string[] chuckInstances = { "spatial_chuck", "spatial_chuck_2", "spatial_chuck_b", "spatial_chuck_b2" };

    void Start()
    {

        Chuck.Manager.Initialize(mixerWithChuck, chuckInstances[samplePlayerID]);

        switch (samplePlayerID)
        {

            case 0:
                Chuck.Manager.RunCode(chuckInstances[samplePlayerID],
                    @"

                    0 => global int reboot;
                    0 => global int prev;
                    0 => global int next;
                    0 => global int back;
                    0 => global int rand;
                    0 => global int swap;
                    [0,0] @=> global int jump[];
                    0 => global int revHold;
                    0 => global int reverse;
                    0 => global int stepMode;
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
                    global int randomSelectVals[16];
                    global int randomPositionVals[16];
                    0 => global int sampdex;
                    0 => global int quit;

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
                        
                        fun int getReverse ()
                        {
                            if (revHold) {
                                if (reverse) { return 1; }
                                else { return 0; }
                            }
                            else {
                                if (reverse) {
                                    0 => reverse;
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
                        
                        1 => int opMode;
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
                                randomSelectVals[i % 16] => seltemp[i];
                            }
                            for (0 => int i; i < postemp.cap(); i++) {
                                randomPositionVals[i % 16] => postemp[i];
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

                    SndBuf2 amen;
                    LPF lowpass[2];
                    HPF hipass[2];
                    PitShift pitchShift[2];
                    DelayL leftdelay;
                    DelayL rightdelay;
                    Gain left;
                    Gain right;
                    Gain leftsampler;
                    Gain rightsampler;
                    Gain leftsum;
                    Gain rightsum;
                    Gain leftdelaygain;
                    Gain rightdelaygain;
                    float leftlevel;
                    float rightlevel;
                    float loudest;
                    float leftdelaytime;
                    float rightdelaytime;
                    0.5 => float gainamount;
                    0.7 => float maxdelay; // in milliseconds

                    Sequencer seq;
                    Shred mainShreds[10];
                    int shredCount;
                    global int currentSample;
                    0 => int flush;
                    0 => int allsamples;
                    0 => int playerID;
                    1.0 => float playerGain;

                    // spatializer input
                    adc.chan(0) => leftsampler => left;
                    adc.chan(1) => rightsampler => right;

                    // sample input
                    amen.chan(0) => lowpass[0] => hipass[0] => pitchShift[0] => left;
                    amen.chan(1) => lowpass[1] => hipass[1] => pitchShift[1] => right;

                    // calculate global tempo
                    me.dir() + ""/0/loops/0.wav"" => amen.read;
                    amen.samples() +=> allsamples;
                    amen.samples() => amen.pos;
                    allsamples/2 => int bar;
                    bar/4 => int beat;

                    // multiply sound file input with spatializer input
                    3 => left.op;
                    3 => right.op;

                    // send to final sum gains
                    left => leftsum  => dac.chan(0);
                    right => rightsum  => dac.chan(1);
                    gainamount => left.gain;
                    gainamount => right.gain;

                    // set up filters & pitch shift
                    20000.0 => lowpass[0].freq => lowpass[1].freq;
                    20.0 => hipass[0].freq => hipass[1].freq;
                    1.9 => lowpass[0].Q => hipass[0].Q;
                    2.2 => lowpass[1].Q => hipass[1].Q;
                    0.0 => pitchShift[0].mix => pitchShift[1].mix;
                    0.5 => pitchShift[0].shift => pitchShift[1].shift;

                    // modifications to rate require inverse ratio as multiplier to tempo
                    [0.8,1.0,1.25] @=> float rate[];
                    [1.25,1.0,0.8] @=> float tempoMod[];
                    0 => int ratedex;

                    fun void playAmen(int id, float gain, float rate, float tempo)
                    {
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
                            selector => currentSample;
                            me.dir() + ""/"" + sampdex + ""/loops/"" + selector + "".wav"" => amen.read;
                            position => amen.pos;
                            gain => amen.gain;
                            if (seq.kb.getReverse()) { -rate => amen.rate; }
                            else { rate => amen.rate; }
                            
                            seq.kb.getSweep() => int sweepDir;
                            seq.kb.getVariance() => int varDir;
                            seq.kb.getStutter() => int stutMode;

                            now => time start;
                            now + tempo::samp => time later;
                            tempo / (Math.pow(2,stutMode)) => float stutLength;

                            if ((varDir < 0) && (sweepDir < 0)) {
                                if (stutMode > 0) {
                                    while (now < later) {
                                        position => amen.pos;
                                        stutLength::samp => now;
                                    }
                                    0 => stutter;
                                }
                                else { tempo::samp => now; }
                            }

                            else {
                                rate => float rateVariance;
                                -0.00005 => float varInc;
                                if (rate < 0) { -varInc => varInc; }
                                varInc * (id + 1) => varInc;

                                20000.0 => float lpFreq;
                                20.0 => float hpFreq;
                                0.33 * (Math.pow(2,id)) => float lpInc;
                                0.005 * (Math.pow(2,id)) => float hpInc;

                                if (varDir == 1) {
                                    if (seq.kb.getReverse()) { -rateVariance => rateVariance; }
                                    rateVariance * 2 => amen.rate;
                                    1.0 => pitchShift[0].mix => pitchShift[1].mix;
                                }

                                while (now < later) {
                                    if (varDir == 0) {
                                        varInc +=> rateVariance;
                                        rateVariance => amen.rate;
                                    }
                                    if (sweepDir >= 0) {
                                        if (sweepDir) {
                                            0.00005 +=> hpInc;
                                            hpInc +=> hpFreq;
                                            if (hpFreq < 20000.0) {
                                                hpFreq => hipass[0].freq => hipass[1].freq;
                                            }
                                        }
                                        else {
                                            0.000005 -=> lpInc;
                                            lpInc -=> lpFreq;
                                            if (lpFreq > 20.0) {
                                                lpFreq => lowpass[0].freq => lowpass[1].freq;
                                            }
                                        }
                                    }
                                    1::samp => now;
                                    if (stutMode > 0) {
                                        if ((now - start) > stutLength::samp) {
                                            position => amen.pos;
                                            now => start;
                                        }
                                        0 => stutter;
                                    }
                                }
                                20000.0 => lowpass[0].freq => lowpass[1].freq;
                                20.0 => hipass[0].freq => hipass[1].freq;
                            }
                            
                            amen.samples() => amen.pos;
                            
                            if (seq.kb.getReboot() || flush) {
                                1 => flush;
                                shredCount--;
                                me.exit();
                            }
                        }
                    }

                    fun void playSampleStealer(int id, float gain, float rate, float tempo)
                    {
                        while (true)
                        {
                            int selector;
                            if (seq.opMode) { seq.getNextSelect(id) => selector; }
                            else { seq.getNextSelect() => selector; }

                            if (selector == currentSample)
                            {
                                gain => amen.gain;
                                tempo::samp => now;
                                playerGain => amen.gain;
                            }
                            else { tempo::samp => now; }
                            
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
                            chout <= ""Current sequence:"";
                            chout <= IO.newline();
                            s.printSelectArray();
                            s.printPositionArray();
                            chout <= IO.newline();
                            0.5::second => now; // can refine display refresh period to taste
                            
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
                                        playerGain => amen.gain;
                                        (duration + adjustment)::samp => now;
                                        0 => gateSwitch;
                                    }
                                    else {
                                        0 => amen.gain;
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
                        spork ~ playAmen(playerID, playerGain, rate[ratedex], (tempoMod[ratedex] * bar)) @=> mainShreds[0];
                        spork ~ playSampleStealer(1, 0.0, rate[ratedex], (tempoMod[ratedex] * (bar/2))) @=> mainShreds[1];
                        spork ~ playSampleStealer(2, 0.0, rate[ratedex], (tempoMod[ratedex] * beat)) @=> mainShreds[2];
                        spork ~ playSampleStealer(3, 0.0, rate[ratedex], (tempoMod[ratedex] * (beat/2))) @=> mainShreds[3];
                        spork ~ printArrays(seq) @=> mainShreds[4];
                        spork ~ updatePlaylist() @=> mainShreds[5];
                        spork ~ mixItUp() @=> mainShreds[6];
                        spork ~ speedListener() @=> mainShreds[7];
                        spork ~ stepModeListener() @=> mainShreds[8];
                        spork ~ gateListener() @=> mainShreds[9];
                        
                        10 => shredCount;
                        0 => flush;
                    }

                    fun void resetListener() {
                        while( true ) {
                            if (quit) { break; }
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

                    fun void amplitudeTracker() {
                        while(true) {
                            leftsampler.last() => leftlevel => loudest;
                            rightsampler.last() => rightlevel;
                            if (rightlevel > loudest) { rightlevel => loudest; }
                            1::ms => now;
                        }
                    }

                    fun void delayDriver() {
                        while(true) {
                            maxdelay * leftlevel => leftdelaytime;
                            maxdelay * rightlevel => rightdelaytime;
                            leftdelaytime::ms => leftdelay.delay;
                            rightdelaytime::ms => rightdelay.delay;
                            1::ms => now;
                        }
                    }

                    fun void amplitudeDriver() {
                        while(true) {
                            gainamount * loudest => leftdelaygain.gain => rightdelaygain.gain;
                            1::ms => now;
                        }
                    }

                    rollOut();

                    // delay sidechain
                    hipass[0] => leftdelay => leftdelaygain => rightsum; // switch channels to
                    hipass[1] => rightdelay => rightdelaygain => leftsum; // normalize effect
                    (maxdelay * 10)::ms => leftdelay.max => leftdelay.delay;
                    (maxdelay * 10)::ms => rightdelay.max => rightdelay.delay;
                    gainamount => leftdelaygain.gain;
                    gainamount => rightdelaygain.gain;

                    Shred helperShreds[3];
                    spork ~ amplitudeTracker() @=> helperShreds[0];
                    spork ~ delayDriver() @=> helperShreds[1];
                    spork ~ amplitudeDriver() @=> helperShreds[2];

                    resetListener();

                    for (0 => int i; i < mainShreds.size(); i++) {
                        mainShreds[i].exit();
                    }

                    for (0 => int i; i < helperShreds.size(); i++) {
                        helperShreds[i].exit();
                    }

                    me.exit();

                    "
                );
                break;

            case 1:
                Chuck.Manager.RunCode(chuckInstances[samplePlayerID],
                    @"

                    0 => global int reboot;
                    0 => global int prev;
                    0 => global int next;
                    0 => global int back;
                    0 => global int rand;
                    0 => global int swap;
                    [0,0] @=> global int jump[];
                    0 => global int revHold;
                    0 => global int reverse;
                    0 => global int stepMode;
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
                    global int randomSelectVals[16];
                    global int randomPositionVals[16];
                    0 => global int sampdex;
                    0 => global int quit;

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
                        
                        fun int getReverse ()
                        {
                            if (revHold) {
                                if (reverse) { return 1; }
                                else { return 0; }
                            }
                            else {
                                if (reverse) {
                                    0 => reverse;
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
                        
                        1 => int opMode;
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
                                randomSelectVals[i % 16] => seltemp[i];
                            }
                            for (0 => int i; i < postemp.cap(); i++) {
                                randomPositionVals[i % 16] => postemp[i];
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
                        
                    }

                    SndBuf2 amen;
                    LPF lowpass[2];
                    HPF hipass[2];
                    PitShift pitchShift[2];
                    DelayL leftdelay;
                    DelayL rightdelay;
                    Gain left;
                    Gain right;
                    Gain leftsampler;
                    Gain rightsampler;
                    Gain leftsum;
                    Gain rightsum;
                    Gain leftdelaygain;
                    Gain rightdelaygain;
                    float leftlevel;
                    float rightlevel;
                    float loudest;
                    float leftdelaytime;
                    float rightdelaytime;
                    0.5 => float gainamount;
                    0.7 => float maxdelay; // in milliseconds

                    Sequencer seq;
                    Shred mainShreds[9];
                    int shredCount;
                    global int currentSample;
                    0 => int flush;
                    0 => int allsamples;
                    1 => int playerID;
                    0.9 => float playerGain;

                    // spatializer input
                    adc.chan(0) => leftsampler => left;
                    adc.chan(1) => rightsampler => right;

                    // sample input
                    amen.chan(0) => lowpass[0] => hipass[0] => pitchShift[0] => left;
                    amen.chan(1) => lowpass[1] => hipass[1] => pitchShift[1] => right;

                    // calculate global tempo
                    me.dir() + ""/0/loops/0.wav"" => amen.read;
                    amen.samples() +=> allsamples;
                    amen.samples() => amen.pos;
                    allsamples/2 => int bar;
                    bar/4 => int beat;

                    // multiply sound file input with spatializer input
                    3 => left.op;
                    3 => right.op;

                    // send to final sum gains
                    left => leftsum  => dac.chan(0);
                    right => rightsum  => dac.chan(1);
                    gainamount => left.gain;
                    gainamount => right.gain;

                    // set up filters & pitch shift
                    20000.0 => lowpass[0].freq => lowpass[1].freq;
                    20.0 => hipass[0].freq => hipass[1].freq;
                    1.9 => lowpass[0].Q => hipass[0].Q;
                    2.2 => lowpass[1].Q => hipass[1].Q;
                    0.0 => pitchShift[0].mix => pitchShift[1].mix;
                    0.5 => pitchShift[0].shift => pitchShift[1].shift;

                    // modifications to rate require inverse ratio as multiplier to tempo
                    [0.8,1.0,1.25] @=> float rate[];
                    [1.25,1.0,0.8] @=> float tempoMod[];
                    0 => int ratedex;

                    fun void playAmen(int id, float gain, float rate, float tempo)
                    {
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
                            selector => currentSample;
                            me.dir() + ""/"" + sampdex + ""/loops/"" + selector + "".wav"" => amen.read;
                            position => amen.pos;
                            gain => amen.gain;
                            if (seq.kb.getReverse()) { -rate => amen.rate; }
                            else { rate => amen.rate; }
                            
                            seq.kb.getSweep() => int sweepDir;
                            seq.kb.getVariance() => int varDir;
                            seq.kb.getStutter() => int stutMode;

                            now => time start;
                            now + tempo::samp => time later;
                            tempo / (Math.pow(2,stutMode)) => float stutLength;

                            if ((varDir < 0) && (sweepDir < 0)) {
                                if (stutMode > 0) {
                                    while (now < later) {
                                        position => amen.pos;
                                        stutLength::samp => now;
                                    }
                                    0 => stutter;
                                }
                                else { tempo::samp => now; }
                            }

                            else {
                                rate => float rateVariance;
                                -0.00005 => float varInc;
                                if (rate < 0) { -varInc => varInc; }
                                varInc * (id + 1) => varInc;

                                20000.0 => float lpFreq;
                                20.0 => float hpFreq;
                                0.33 * (Math.pow(2,id)) => float lpInc;
                                0.005 * (Math.pow(2,id)) => float hpInc;

                                if (varDir == 1) {
                                    if (seq.kb.getReverse()) { -rateVariance => rateVariance; }
                                    rateVariance * 2 => amen.rate;
                                    1.0 => pitchShift[0].mix => pitchShift[1].mix;
                                }

                                while (now < later) {
                                    if (varDir == 0) {
                                        varInc +=> rateVariance;
                                        rateVariance => amen.rate;
                                    }
                                    if (sweepDir >= 0) {
                                        if (sweepDir) {
                                            0.00005 +=> hpInc;
                                            hpInc +=> hpFreq;
                                            if (hpFreq < 20000.0) {
                                                hpFreq => hipass[0].freq => hipass[1].freq;
                                            }
                                        }
                                        else {
                                            0.000005 -=> lpInc;
                                            lpInc -=> lpFreq;
                                            if (lpFreq > 20.0) {
                                                lpFreq => lowpass[0].freq => lowpass[1].freq;
                                            }
                                        }
                                    }
                                    1::samp => now;
                                    if (stutMode > 0) {
                                        if ((now - start) > stutLength::samp) {
                                            position => amen.pos;
                                            now => start;
                                        }
                                        0 => stutter;
                                    }
                                }
                                20000.0 => lowpass[0].freq => lowpass[1].freq;
                                20.0 => hipass[0].freq => hipass[1].freq;
                            }
                            
                            amen.samples() => amen.pos;
                            
                            if (seq.kb.getReboot() || flush) {
                                1 => flush;
                                shredCount--;
                                me.exit();
                            }
                        }
                    }

                    fun void playSampleStealer(int id, float gain, float rate, float tempo)
                    {
                        while (true)
                        {
                            int selector;
                            if (seq.opMode) { seq.getNextSelect(id) => selector; }
                            else { seq.getNextSelect() => selector; }

                            if (selector == currentSample)
                            {
                                gain => amen.gain;
                                tempo::samp => now;
                                playerGain => amen.gain;
                            }
                            else { tempo::samp => now; }
                            
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
                                        playerGain => amen.gain;
                                        (duration + adjustment)::samp => now;
                                        0 => gateSwitch;
                                    }
                                    else {
                                        0 => amen.gain;
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
                        spork ~ playSampleStealer(0, 0.0, rate[ratedex], (tempoMod[ratedex] * bar)) @=> mainShreds[0];
                        spork ~ playAmen(playerID, playerGain, rate[ratedex], (tempoMod[ratedex] * (bar/2))) @=> mainShreds[1];
                        spork ~ playSampleStealer(2, 0.0, rate[ratedex], (tempoMod[ratedex] * beat)) @=> mainShreds[2];
                        spork ~ playSampleStealer(3, 0.0, rate[ratedex], (tempoMod[ratedex] * (beat/2))) @=> mainShreds[3];
                        spork ~ updatePlaylist() @=> mainShreds[4];
                        spork ~ mixItUp() @=> mainShreds[5];
                        spork ~ speedListener() @=> mainShreds[6];
                        spork ~ stepModeListener() @=> mainShreds[7];
                        spork ~ gateListener() @=> mainShreds[8];
                        
                        9 => shredCount;
                        0 => flush;
                    }

                    fun void resetListener() {
                        while( true ) {
                            if (quit) { break; }
                            (tempoMod[ratedex] * (beat/2))::samp => now;
                            if (shredCount < 9) {
                                mainShreds[0].exit();
                                mainShreds[1].exit();
                                mainShreds[2].exit();
                                mainShreds[3].exit();
                                mainShreds[4].exit();
                                mainShreds[5].exit();
                                mainShreds[6].exit();
                                mainShreds[7].exit();
                                mainShreds[8].exit();
                                rollOut();
                            }
                        }
                    }

                    fun void amplitudeTracker() {
                        while(true) {
                            leftsampler.last() => leftlevel => loudest;
                            rightsampler.last() => rightlevel;
                            if (rightlevel > loudest) { rightlevel => loudest; }
                            1::ms => now;
                        }
                    }

                    fun void delayDriver() {
                        while(true) {
                            maxdelay * leftlevel => leftdelaytime;
                            maxdelay * rightlevel => rightdelaytime;
                            leftdelaytime::ms => leftdelay.delay;
                            rightdelaytime::ms => rightdelay.delay;
                            1::ms => now;
                        }
                    }

                    fun void amplitudeDriver() {
                        while(true) {
                            gainamount * loudest => leftdelaygain.gain => rightdelaygain.gain;
                            1::ms => now;
                        }
                    }

                    rollOut();

                    // delay sidechain
                    hipass[0] => leftdelay => leftdelaygain => rightsum; // switch channels to
                    hipass[1] => rightdelay => rightdelaygain => leftsum; // normalize effect
                    (maxdelay * 10)::ms => leftdelay.max => leftdelay.delay;
                    (maxdelay * 10)::ms => rightdelay.max => rightdelay.delay;
                    gainamount => leftdelaygain.gain;
                    gainamount => rightdelaygain.gain;

                    Shred helperShreds[3];
                    spork ~ amplitudeTracker() @=> helperShreds[0];
                    spork ~ delayDriver() @=> helperShreds[1];
                    spork ~ amplitudeDriver() @=> helperShreds[2];

                    resetListener();

                    for (0 => int i; i < mainShreds.size(); i++) {
                        mainShreds[i].exit();
                    }

                    for (0 => int i; i < helperShreds.size(); i++) {
                        helperShreds[i].exit();
                    }

                    me.exit();

                    "
                );
                break;

            case 2:
                Chuck.Manager.RunCode(chuckInstances[samplePlayerID],
                    @"

                    0 => global int reboot;
                    0 => global int prev;
                    0 => global int next;
                    0 => global int back;
                    0 => global int rand;
                    0 => global int swap;
                    [0,0] @=> global int jump[];
                    0 => global int revHold;
                    0 => global int reverse;
                    0 => global int stepMode;
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
                    global int randomSelectVals[16];
                    global int randomPositionVals[16];
                    0 => global int sampdex;
                    0 => global int quit;

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
                        
                        fun int getReverse ()
                        {
                            if (revHold) {
                                if (reverse) { return 1; }
                                else { return 0; }
                            }
                            else {
                                if (reverse) {
                                    0 => reverse;
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
                        
                        1 => int opMode;
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
                                randomSelectVals[i % 16] => seltemp[i];
                            }
                            for (0 => int i; i < postemp.cap(); i++) {
                                randomPositionVals[i % 16] => postemp[i];
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
                        
                    }

                    SndBuf2 amen;
                    LPF lowpass[2];
                    HPF hipass[2];
                    PitShift pitchShift[2];
                    DelayL leftdelay;
                    DelayL rightdelay;
                    Gain left;
                    Gain right;
                    Gain leftsampler;
                    Gain rightsampler;
                    Gain leftsum;
                    Gain rightsum;
                    Gain leftdelaygain;
                    Gain rightdelaygain;
                    float leftlevel;
                    float rightlevel;
                    float loudest;
                    float leftdelaytime;
                    float rightdelaytime;
                    0.5 => float gainamount;
                    0.7 => float maxdelay; // in milliseconds

                    Sequencer seq;
                    Shred mainShreds[9];
                    int shredCount;
                    global int currentSample;
                    0 => int flush;
                    0 => int allsamples;
                    2 => int playerID;
                    0.75 => float playerGain;

                    // spatializer input
                    adc.chan(0) => leftsampler => left;
                    adc.chan(1) => rightsampler => right;

                    // sample input
                    amen.chan(0) => lowpass[0] => hipass[0] => pitchShift[0] => left;
                    amen.chan(1) => lowpass[1] => hipass[1] => pitchShift[1] => right;

                    // calculate global tempo
                    me.dir() + ""/0/loops/0.wav"" => amen.read;
                    amen.samples() +=> allsamples;
                    amen.samples() => amen.pos;
                    allsamples/2 => int bar;
                    bar/4 => int beat;

                    // multiply sound file input with spatializer input
                    3 => left.op;
                    3 => right.op;

                    // send to final sum gains
                    left => leftsum  => dac.chan(0);
                    right => rightsum  => dac.chan(1);
                    gainamount => left.gain;
                    gainamount => right.gain;

                    // set up filters & pitch shift
                    20000.0 => lowpass[0].freq => lowpass[1].freq;
                    20.0 => hipass[0].freq => hipass[1].freq;
                    1.9 => lowpass[0].Q => hipass[0].Q;
                    2.2 => lowpass[1].Q => hipass[1].Q;
                    0.0 => pitchShift[0].mix => pitchShift[1].mix;
                    0.5 => pitchShift[0].shift => pitchShift[1].shift;

                    // modifications to rate require inverse ratio as multiplier to tempo
                    [0.8,1.0,1.25] @=> float rate[];
                    [1.25,1.0,0.8] @=> float tempoMod[];
                    0 => int ratedex;

                    fun void playAmen(int id, float gain, float rate, float tempo)
                    {
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
                            selector => currentSample;
                            me.dir() + ""/"" + sampdex + ""/loops/"" + selector + "".wav"" => amen.read;
                            position => amen.pos;
                            gain => amen.gain;
                            if (seq.kb.getReverse()) { -rate => amen.rate; }
                            else { rate => amen.rate; }
                            
                            seq.kb.getSweep() => int sweepDir;
                            seq.kb.getVariance() => int varDir;
                            seq.kb.getStutter() => int stutMode;

                            now => time start;
                            now + tempo::samp => time later;
                            tempo / (Math.pow(2,stutMode)) => float stutLength;

                            if ((varDir < 0) && (sweepDir < 0)) {
                                if (stutMode > 0) {
                                    while (now < later) {
                                        position => amen.pos;
                                        stutLength::samp => now;
                                    }
                                    0 => stutter;
                                }
                                else { tempo::samp => now; }
                            }

                            else {
                                rate => float rateVariance;
                                -0.00005 => float varInc;
                                if (rate < 0) { -varInc => varInc; }
                                varInc * (id + 1) => varInc;

                                20000.0 => float lpFreq;
                                20.0 => float hpFreq;
                                0.33 * (Math.pow(2,id)) => float lpInc;
                                0.005 * (Math.pow(2,id)) => float hpInc;

                                if (varDir == 1) {
                                    if (seq.kb.getReverse()) { -rateVariance => rateVariance; }
                                    rateVariance * 2 => amen.rate;
                                    1.0 => pitchShift[0].mix => pitchShift[1].mix;
                                }

                                while (now < later) {
                                    if (varDir == 0) {
                                        varInc +=> rateVariance;
                                        rateVariance => amen.rate;
                                    }
                                    if (sweepDir >= 0) {
                                        if (sweepDir) {
                                            0.00005 +=> hpInc;
                                            hpInc +=> hpFreq;
                                            if (hpFreq < 20000.0) {
                                                hpFreq => hipass[0].freq => hipass[1].freq;
                                            }
                                        }
                                        else {
                                            0.000005 -=> lpInc;
                                            lpInc -=> lpFreq;
                                            if (lpFreq > 20.0) {
                                                lpFreq => lowpass[0].freq => lowpass[1].freq;
                                            }
                                        }
                                    }
                                    1::samp => now;
                                    if (stutMode > 0) {
                                        if ((now - start) > stutLength::samp) {
                                            position => amen.pos;
                                            now => start;
                                        }
                                        0 => stutter;
                                    }
                                }
                                20000.0 => lowpass[0].freq => lowpass[1].freq;
                                20.0 => hipass[0].freq => hipass[1].freq;
                            }
                            
                            amen.samples() => amen.pos;
                            
                            if (seq.kb.getReboot() || flush) {
                                1 => flush;
                                shredCount--;
                                me.exit();
                            }
                        }
                    }

                    fun void playSampleStealer(int id, float gain, float rate, float tempo)
                    {
                        while (true)
                        {
                            int selector;
                            if (seq.opMode) { seq.getNextSelect(id) => selector; }
                            else { seq.getNextSelect() => selector; }

                            if (selector == currentSample)
                            {
                                gain => amen.gain;
                                tempo::samp => now;
                                playerGain => amen.gain;
                            }
                            else { tempo::samp => now; }
                            
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
                                        playerGain => amen.gain;
                                        (duration + adjustment)::samp => now;
                                        0 => gateSwitch;
                                    }
                                    else {
                                        0 => amen.gain;
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
                        spork ~ playSampleStealer(0, 0.0, rate[ratedex], (tempoMod[ratedex] * bar)) @=> mainShreds[0];
                        spork ~ playSampleStealer(1, 0.0, rate[ratedex], (tempoMod[ratedex] * (bar/2))) @=> mainShreds[1];
                        spork ~ playAmen(playerID, playerGain, rate[ratedex], (tempoMod[ratedex] * beat)) @=> mainShreds[2];
                        spork ~ playSampleStealer(3, 0.0, rate[ratedex], (tempoMod[ratedex] * (beat/2))) @=> mainShreds[3];
                        spork ~ updatePlaylist() @=> mainShreds[4];
                        spork ~ mixItUp() @=> mainShreds[5];
                        spork ~ speedListener() @=> mainShreds[6];
                        spork ~ stepModeListener() @=> mainShreds[7];
                        spork ~ gateListener() @=> mainShreds[8];
                        
                        9 => shredCount;
                        0 => flush;
                    }

                    fun void resetListener() {
                        while( true ) {
                            if (quit) { break; }
                            (tempoMod[ratedex] * (beat/2))::samp => now;
                            if (shredCount < 9) {
                                mainShreds[0].exit();
                                mainShreds[1].exit();
                                mainShreds[2].exit();
                                mainShreds[3].exit();
                                mainShreds[4].exit();
                                mainShreds[5].exit();
                                mainShreds[6].exit();
                                mainShreds[7].exit();
                                mainShreds[8].exit();
                                rollOut();
                            }
                        }
                    }

                    fun void amplitudeTracker() {
                        while(true) {
                            leftsampler.last() => leftlevel => loudest;
                            rightsampler.last() => rightlevel;
                            if (rightlevel > loudest) { rightlevel => loudest; }
                            1::ms => now;
                        }
                    }

                    fun void delayDriver() {
                        while(true) {
                            maxdelay * leftlevel => leftdelaytime;
                            maxdelay * rightlevel => rightdelaytime;
                            leftdelaytime::ms => leftdelay.delay;
                            rightdelaytime::ms => rightdelay.delay;
                            1::ms => now;
                        }
                    }

                    fun void amplitudeDriver() {
                        while(true) {
                            gainamount * loudest => leftdelaygain.gain => rightdelaygain.gain;
                            1::ms => now;
                        }
                    }

                    rollOut();

                    // delay sidechain
                    hipass[0] => leftdelay => leftdelaygain => rightsum; // switch channels to
                    hipass[1] => rightdelay => rightdelaygain => leftsum; // normalize effect
                    (maxdelay * 10)::ms => leftdelay.max => leftdelay.delay;
                    (maxdelay * 10)::ms => rightdelay.max => rightdelay.delay;
                    gainamount => leftdelaygain.gain;
                    gainamount => rightdelaygain.gain;

                    Shred helperShreds[3];
                    spork ~ amplitudeTracker() @=> helperShreds[0];
                    spork ~ delayDriver() @=> helperShreds[1];
                    spork ~ amplitudeDriver() @=> helperShreds[2];

                    resetListener();

                    for (0 => int i; i < mainShreds.size(); i++) {
                        mainShreds[i].exit();
                    }

                    for (0 => int i; i < helperShreds.size(); i++) {
                        helperShreds[i].exit();
                    }

                    me.exit();

                    "
                );
                break;

            case 3:
                Chuck.Manager.RunCode(chuckInstances[samplePlayerID],
                    @"

                    0 => global int reboot;
                    0 => global int prev;
                    0 => global int next;
                    0 => global int back;
                    0 => global int rand;
                    0 => global int swap;
                    [0,0] @=> global int jump[];
                    0 => global int revHold;
                    0 => global int reverse;
                    0 => global int stepMode;
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
                    global int randomSelectVals[16];
                    global int randomPositionVals[16];
                    0 => global int sampdex;
                    0 => global int quit;

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
                        
                        fun int getReverse ()
                        {
                            if (revHold) {
                                if (reverse) { return 1; }
                                else { return 0; }
                            }
                            else {
                                if (reverse) {
                                    0 => reverse;
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
                        
                        1 => int opMode;
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
                                randomSelectVals[i % 16] => seltemp[i];
                            }
                            for (0 => int i; i < postemp.cap(); i++) {
                                randomPositionVals[i % 16] => postemp[i];
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
                        
                    }

                    SndBuf2 amen;
                    LPF lowpass[2];
                    HPF hipass[2];
                    PitShift pitchShift[2];
                    DelayL leftdelay;
                    DelayL rightdelay;
                    Gain left;
                    Gain right;
                    Gain leftsampler;
                    Gain rightsampler;
                    Gain leftsum;
                    Gain rightsum;
                    Gain leftdelaygain;
                    Gain rightdelaygain;
                    float leftlevel;
                    float rightlevel;
                    float loudest;
                    float leftdelaytime;
                    float rightdelaytime;
                    0.5 => float gainamount;
                    0.7 => float maxdelay; // in milliseconds

                    Sequencer seq;
                    Shred mainShreds[9];
                    int shredCount;
                    global int currentSample;
                    0 => int flush;
                    0 => int allsamples;
                    3 => int playerID;
                    0.5 => float playerGain;

                    // spatializer input
                    adc.chan(0) => leftsampler => left;
                    adc.chan(1) => rightsampler => right;

                    // sample input
                    amen.chan(0) => lowpass[0] => hipass[0] => pitchShift[0] => left;
                    amen.chan(1) => lowpass[1] => hipass[1] => pitchShift[1] => right;

                    // calculate global tempo
                    me.dir() + ""/0/loops/0.wav"" => amen.read;
                    amen.samples() +=> allsamples;
                    amen.samples() => amen.pos;
                    allsamples/2 => int bar;
                    bar/4 => int beat;

                    // multiply sound file input with spatializer input
                    3 => left.op;
                    3 => right.op;

                    // send to final sum gains
                    left => leftsum  => dac.chan(0);
                    right => rightsum  => dac.chan(1);
                    gainamount => left.gain;
                    gainamount => right.gain;

                    // set up filters & pitch shift
                    20000.0 => lowpass[0].freq => lowpass[1].freq;
                    20.0 => hipass[0].freq => hipass[1].freq;
                    1.9 => lowpass[0].Q => hipass[0].Q;
                    2.2 => lowpass[1].Q => hipass[1].Q;
                    0.0 => pitchShift[0].mix => pitchShift[1].mix;
                    0.5 => pitchShift[0].shift => pitchShift[1].shift;

                    // modifications to rate require inverse ratio as multiplier to tempo
                    [0.8,1.0,1.25] @=> float rate[];
                    [1.25,1.0,0.8] @=> float tempoMod[];
                    0 => int ratedex;

                    fun void playAmen(int id, float gain, float rate, float tempo)
                    {
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
                            selector => currentSample;
                            me.dir() + ""/"" + sampdex + ""/loops/"" + selector + "".wav"" => amen.read;
                            position => amen.pos;
                            gain => amen.gain;
                            if (seq.kb.getReverse()) { -rate => amen.rate; }
                            else { rate => amen.rate; }
                            
                            seq.kb.getSweep() => int sweepDir;
                            seq.kb.getVariance() => int varDir;
                            seq.kb.getStutter() => int stutMode;

                            now => time start;
                            now + tempo::samp => time later;
                            tempo / (Math.pow(2,stutMode)) => float stutLength;

                            if ((varDir < 0) && (sweepDir < 0)) {
                                if (stutMode > 0) {
                                    while (now < later) {
                                        position => amen.pos;
                                        stutLength::samp => now;
                                    }
                                    0 => stutter;
                                }
                                else { tempo::samp => now; }
                            }

                            else {
                                rate => float rateVariance;
                                -0.00005 => float varInc;
                                if (rate < 0) { -varInc => varInc; }
                                varInc * (id + 1) => varInc;

                                20000.0 => float lpFreq;
                                20.0 => float hpFreq;
                                0.33 * (Math.pow(2,id)) => float lpInc;
                                0.005 * (Math.pow(2,id)) => float hpInc;

                                if (varDir == 1) {
                                    if (seq.kb.getReverse()) { -rateVariance => rateVariance; }
                                    rateVariance * 2 => amen.rate;
                                    1.0 => pitchShift[0].mix => pitchShift[1].mix;
                                }

                                while (now < later) {
                                    if (varDir == 0) {
                                        varInc +=> rateVariance;
                                        rateVariance => amen.rate;
                                    }
                                    if (sweepDir >= 0) {
                                        if (sweepDir) {
                                            0.00005 +=> hpInc;
                                            hpInc +=> hpFreq;
                                            if (hpFreq < 20000.0) {
                                                hpFreq => hipass[0].freq => hipass[1].freq;
                                            }
                                        }
                                        else {
                                            0.000005 -=> lpInc;
                                            lpInc -=> lpFreq;
                                            if (lpFreq > 20.0) {
                                                lpFreq => lowpass[0].freq => lowpass[1].freq;
                                            }
                                        }
                                    }
                                    1::samp => now;
                                    if (stutMode > 0) {
                                        if ((now - start) > stutLength::samp) {
                                            position => amen.pos;
                                            now => start;
                                        }
                                        0 => stutter;
                                    }
                                }
                                20000.0 => lowpass[0].freq => lowpass[1].freq;
                                20.0 => hipass[0].freq => hipass[1].freq;
                            }
                            
                            amen.samples() => amen.pos;
                            
                            if (seq.kb.getReboot() || flush) {
                                1 => flush;
                                shredCount--;
                                me.exit();
                            }
                        }
                    }

                    fun void playSampleStealer(int id, float gain, float rate, float tempo)
                    {
                        while (true)
                        {
                            int selector;
                            if (seq.opMode) { seq.getNextSelect(id) => selector; }
                            else { seq.getNextSelect() => selector; }

                            if (selector == currentSample)
                            {
                                gain => amen.gain;
                                tempo::samp => now;
                                playerGain => amen.gain;
                            }
                            else { tempo::samp => now; }
                            
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
                                        playerGain => amen.gain;
                                        (duration + adjustment)::samp => now;
                                        0 => gateSwitch;
                                    }
                                    else {
                                        0 => amen.gain;
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
                        spork ~ playSampleStealer(0, 0.0, rate[ratedex], (tempoMod[ratedex] * bar)) @=> mainShreds[0];
                        spork ~ playSampleStealer(1, 0.0, rate[ratedex], (tempoMod[ratedex] * (bar/2))) @=> mainShreds[1];
                        spork ~ playSampleStealer(2, 0.0, rate[ratedex], (tempoMod[ratedex] * beat)) @=> mainShreds[2];
                        spork ~ playAmen(playerID, playerGain, rate[ratedex], (tempoMod[ratedex] * (beat/2))) @=> mainShreds[3];
                        spork ~ updatePlaylist() @=> mainShreds[4];
                        spork ~ mixItUp() @=> mainShreds[5];
                        spork ~ speedListener() @=> mainShreds[6];
                        spork ~ stepModeListener() @=> mainShreds[7];
                        spork ~ gateListener() @=> mainShreds[8];
                        
                        9 => shredCount;
                        0 => flush;
                    }

                    fun void resetListener() {
                        while( true ) {
                            if (quit) { break; }
                            (tempoMod[ratedex] * (beat/2))::samp => now;
                            if (shredCount < 9) {
                                mainShreds[0].exit();
                                mainShreds[1].exit();
                                mainShreds[2].exit();
                                mainShreds[3].exit();
                                mainShreds[4].exit();
                                mainShreds[5].exit();
                                mainShreds[6].exit();
                                mainShreds[7].exit();
                                mainShreds[8].exit();
                                rollOut();
                            }
                        }
                    }

                    fun void amplitudeTracker() {
                        while(true) {
                            leftsampler.last() => leftlevel => loudest;
                            rightsampler.last() => rightlevel;
                            if (rightlevel > loudest) { rightlevel => loudest; }
                            1::ms => now;
                        }
                    }

                    fun void delayDriver() {
                        while(true) {
                            maxdelay * leftlevel => leftdelaytime;
                            maxdelay * rightlevel => rightdelaytime;
                            leftdelaytime::ms => leftdelay.delay;
                            rightdelaytime::ms => rightdelay.delay;
                            1::ms => now;
                        }
                    }

                    fun void amplitudeDriver() {
                        while(true) {
                            gainamount * loudest => leftdelaygain.gain => rightdelaygain.gain;
                            1::ms => now;
                        }
                    }

                    global Event notifier;

                    fun void syncSelectVals() {
                        while( true ) {
                            notifier.broadcast();
                            (tempoMod[ratedex] * (beat/2))::samp => now;
                        }
                    }

                    rollOut();

                    // delay sidechain
                    hipass[0] => leftdelay => leftdelaygain => rightsum; // switch channels to
                    hipass[1] => rightdelay => rightdelaygain => leftsum; // normalize effect
                    (maxdelay * 10)::ms => leftdelay.max => leftdelay.delay;
                    (maxdelay * 10)::ms => rightdelay.max => rightdelay.delay;
                    gainamount => leftdelaygain.gain;
                    gainamount => rightdelaygain.gain;

                    Shred helperShreds[4];
                    spork ~ amplitudeTracker() @=> helperShreds[0];
                    spork ~ delayDriver() @=> helperShreds[1];
                    spork ~ amplitudeDriver() @=> helperShreds[2];
                    spork ~ syncSelectVals() @=> helperShreds[3];

                    resetListener();

                    for (0 => int i; i < mainShreds.size(); i++) {
                        mainShreds[i].exit();
                    }

                    for (0 => int i; i < helperShreds.size(); i++) {
                        helperShreds[i].exit();
                    }

                    me.exit();

                    "
                );
                break;

            default:
                Debug.LogError("Sample Player ID must be 0 - 3");
                break;

        }

        selectCallback = Chuck.CreateGetIntCallback(GetSelectValue);
        syncSelectCallback = Chuck.CreateVoidCallback(SyncSelectValue);
        Chuck.Manager.StartListeningForChuckEvent(chuckInstances[3], "notifier", syncSelectCallback);

        xPos = transform.position.x;
        yPos = transform.position.y;
        zPos = transform.position.z;

    }

    void GetSelectValue(long sVal)
    {
        selectValue = (int)sVal;
    }

    void SyncSelectValue()
    {
        Chuck.Manager.GetInt(chuckInstances[samplePlayerID], "currentSample", selectCallback);
        synced = true;
    }

    void Update()
    {
        if (synced)
        {
            switch (samplePlayerID)
            {
                case 0:
                    xPos = selectValue * 0.8f;
                    newPosition = new Vector3(xPos - 6, yPos, zPos);
                    break;

                case 1:
                    zPos = selectValue * 0.8f;
                    newPosition = new Vector3(xPos, yPos, zPos - 6);
                    break;

                case 2:
                    xPos = selectValue * 0.8f;
                    newPosition = new Vector3(xPos - 6, yPos, zPos);
                    break;

                case 3:
                    zPos = selectValue * 0.8f;
                    newPosition = new Vector3(xPos, yPos, zPos - 6);
                    break;

                default:
                    Debug.LogError("Sample Player ID must be 0 - 3");
                    break;
            }
            transform.position = newPosition;
            synced = false;
        }
    }

}
