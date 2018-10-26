class Keyboard {
    
    KBHit kb;
    0 => int reboot;
    0 => int prev;
    0 => int next;
    0 => int back;
    0 => int rand;
    0 => int swap;
    [0,0] @=> int jump[];
    0 => int revHold;
    [0,0,0,0] @=> int reverse[];
    0 => int stepMode;
    0 => int downshift;
    0 => int upshift;
    0 => int sweep;
    0 => int sweepDir;
    0 => int variance;
    0 => int varianceDir;
    0 => int stutter;
    2 => int stutDur;
    0 => int gate;
    3 => int gateDur;
    0 => int gateAdjust;
    0 => int writeSelect;
    0 => int writePosition;
    0 => int record;
    0 => int exit;
    int lastSelectVal;
    int lastPositionVal;
    int tempVal;
    
    fun void listen()
    {
        while( true ) {
            
            kb => now;
            while( kb.more() ) { kb.getchar() => tempVal; }
            
            // pressing escape reboots all main shreds
            if (tempVal == 27) { 1 => reboot; }
            // pressing "[" selects the previous sequence in playlist
            if (tempVal == 91) { 1 => prev; }
            // pressing "]" selects the next sequence in playlist
            if (tempVal == 93) { 1 => next; }
            // pressing delete reverts to the last used sequence
            if (tempVal == 127) { 1 => back; }
            // pressing spacebar creates a randomized sequence
            if (tempVal == 32) { 1 => rand; }
            // pressing tab swaps current sequence into playlist
            if (tempVal == 9) { 1 => swap; }
            // pressing numbers 1 - 8 jumps to that sequence in playlist
            if ((tempVal > 48) && (tempVal < 57)) {
                (tempVal - 48) => jump[1];
                1 => jump[0];
            }
            // pressing "w" toggles auto-hold for reverse playback
            if (tempVal == 119) {
                if (revHold) { 0 => revHold; }
                else { 1 => revHold; }
            }
            // pressing/holding "r" reverses playback for sample player 1
            if (tempVal == 114) {
                if (revHold) {
                    if (reverse[0]) { 0 => reverse[0]; }
                    else { 1 => reverse[0]; }
                }
                else { 1 => reverse[0]; }
            }
            // pressing/holding "t" reverses playback for sample player 2
            if (tempVal == 116) {
                if (revHold) {
                    if (reverse[1]) { 0 => reverse[1]; }
                    else { 1 => reverse[1]; }
                }
                else { 1 => reverse[1]; }
            }
            // pressing/holding "y" reverses playback for sample player 3
            if (tempVal == 121) {
                if (revHold) {
                    if (reverse[2]) { 0 => reverse[2]; }
                    else { 1 => reverse[2]; }
                }
                else { 1 => reverse[2]; }
            }
            // pressing/holding "u" reverses playback for sample player 4
            if (tempVal == 117) {
                if (revHold) {
                    if (reverse[3]) { 0 => reverse[3]; }
                    else { 1 => reverse[3]; }
                }
                else { 1 => reverse[3]; }
            }
            // pressing "q" toggles sequencer's step determination mode
            if (tempVal == 113) { 1 => stepMode; }
            // pressing "z" and "x" switches playback speeds (must reboot)
            if (tempVal == 122) { 1 => downshift; }
            if (tempVal == 120) { 1 => upshift; }
            // pressing "s" toggles filter sweeps in set direction
            if (tempVal == 115) {
                if (sweep) { 0 => sweep; }
                else { 1 => sweep; }
            }
            // pressing ";" sets sweep direction down, "'" sets to up
            if (tempVal == 59) { 0 => sweepDir; }
            if (tempVal == 39) { 1 => sweepDir; }
            // holding "v" performs tempo variance curves in set direction
            if (tempVal == 118) { 1 => variance; }
            // pressing "," sets variance direction down, "." sets to up
            if (tempVal == 44) { 0 => varianceDir; }
            if (tempVal == 46) { 1 => varianceDir; }
            // pressing "/" toggles a stutter of set duration
            if (tempVal == 47) {
                if (stutter) { 0 => stutter; }
                else { 1 => stutter; }
            }
            // pressing 9 or 0 shortens or lengthens stutter duration
            if (tempVal == 57) {
                if (stutDur > 1) { stutDur--; }
            }
            if (tempVal == 48) {
                if (stutDur < 4) { stutDur++; }
            }
            // pressing "\" toggles rhythmic gating of set duration
            if (tempVal == 92) {
                if (gate) { 0 => gate; }
                else { 1 => gate; }
            }
            // pressing "(" or ")" shortens or lengthens gate duration
            if (tempVal == 40) {
                if (gateDur > 1) { gateDur--; }
            }
            if (tempVal == 41) {
                if (gateDur < 4) { gateDur++; }
            }
            // pressing "_" or "+" adjusts audible period for gating
            if (tempVal == 95) {
                if (gateAdjust > -2) { gateAdjust--; }
            }
            if (tempVal == 43) {
                if (gateAdjust < 2) { gateAdjust++; }
            }
            // pressing "=" writes last pressed select value in step
            if (tempVal == 61) { 1 => writeSelect; }
            // pressing "-" writes last pressed position value in step
            if (tempVal == 45) { 1 => writePosition; }
            // pressing letters a - p sets lastSelectVal up to write
            if ((tempVal > 96) && (tempVal < 113)) {
                (tempVal - 97) => lastSelectVal;
            }
            // pressing shift + numbers 1 - 8 sets lastPositionVal up to write
            if (tempVal == 33) { 0 => lastPositionVal; }
            if (tempVal == 64) { 1 => lastPositionVal; }
            if ((tempVal > 34) && (tempVal < 38)) {
                (tempVal - 33) => lastPositionVal;
            }
            if (tempVal == 94) { 5 => lastPositionVal; }
            if (tempVal == 38) { 6 => lastPositionVal; }
            if (tempVal == 42) { 7 => lastPositionVal; }
            // pressing "R" (shift + r) toggles recording
            if (tempVal == 82) { 1 => record; }
            // pressing "O" (shift + o) closes the program
            if (tempVal == 79) { 1 => exit; }
        }
    }
    
    spork ~ listen();
    
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
    
    fun int getRecord ()
    {
        if (record) {
            0 => record;
            return 1;
        }
        else { return 0; }
    }
    
    fun int getExit ()
    {
        return exit;
    }
    
}

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
    // can be tweaked in real time using the step-insert keys ("-" and "=")
    
    0 => int opMode;
    0 => int seldex;
    0 => int posdex;
    [0,0,0,0] @=> int seldexPack[];
    [0,0,0,0] @=> int posdexPack[];
    1 => int scrolldex;
    1 => int lastScrolldex;
    0 => int navFlag;
    1 => int isRecording;
    
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
        chout <= "| ";
        for (0 => int i; i < select.cap(); i++) {
            chout <= select[i] <= " | ";
        }
        if (isRecording) { chout <= " ***Recording*** "; }
        chout <= IO.newline();
    }
    
    fun void printPositionArray()
    {
        chout <= "| ";
        for (0 => int i; i < position.cap(); i++) {
            chout <= position[i] <= " | ";
        }
        if (isRecording) { chout <= " ***Recording*** "; }
        chout <= IO.newline();
    }
    
}

16 => int channels;
SndBuf2 amen[channels];
LPF lowpass[channels*2];
HPF hipass[channels*2];
Pan2 pan[channels];
WvOut2 recordings[4];
0 => int allsamples;
Sequencer seq;

for (0 => int i; i < channels; i++)
{
    // BYOL(bring your own loops), wants 16 of them labeled 0.wav - 15.wav
    me.dir()+"/audiobin/fancy/loops/"+i+".wav" => amen[i].read;
    // all samples must have the same length and tempo
    amen[i].samples() +=> allsamples;
    amen[i].samples() => amen[i].pos;
    10::ms => now;
    
    if (i < 4) {
        amen[i].chan(0) => lowpass[i] => hipass[i] => pan[i].chan(0) => recordings[0].chan(0) => dac.chan(0);
        amen[i].chan(1) => lowpass[i+channels] => hipass[i+channels] => pan[i].chan(1) => recordings[0].chan(1) => dac.chan(1);
    }
    if (i > 3 && i < 6) {
        amen[i].chan(0) => lowpass[i] => hipass[i] => pan[i].chan(0) => recordings[1].chan(0) => dac.chan(0);
        amen[i].chan(1) => lowpass[i+channels] => hipass[i+channels] => pan[i].chan(1) => recordings[1].chan(1) => dac.chan(1);
    }
    if (i > 5 && i < 10) {
        amen[i].chan(0) => lowpass[i] => hipass[i] => pan[i].chan(0) => recordings[2].chan(0) => dac.chan(0);
        amen[i].chan(1) => lowpass[i+channels] => hipass[i+channels] => pan[i].chan(1) => recordings[2].chan(1) => dac.chan(1);
    }
    if (i > 9) {
        amen[i].chan(0) => lowpass[i] => hipass[i] => pan[i].chan(0) => recordings[3].chan(0) => dac.chan(0);
        amen[i].chan(1) => lowpass[i+channels] => hipass[i+channels] => pan[i].chan(1) => recordings[3].chan(1) => dac.chan(1);
    }
    10::ms => now;
    
    20000.0 => lowpass[i].freq;
    20000.0 => lowpass[i+channels].freq;
    20.0 => hipass[i].freq;
    20.0 => hipass[i+channels].freq;
    1.9 => lowpass[i].Q;
    2.2 => lowpass[i+channels].Q;
    1.9 => hipass[i].Q;
    2.2 => hipass[i+channels].Q;
}

for (0 => int i; i < 4; i++) {
    "Track_" + (i+1) => recordings[i].autoPrefix;
    "special:auto" => recordings[i].wavFilename;
    10::ms => now;
    1 => recordings[i].record;
    10::ms => now;
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
                0 => seq.kb.stutter;
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
                0 => seq.kb.stutter;
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
        0.25::second => now; // refine display refresh period here
        
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

spork ~ resetListener() @=> Shred rL;

// main while loop wraps in recordListener & exitListener functionality
while( true ) {
    (tempoMod[ratedex] * (beat/2))::samp => now;
    if (seq.kb.getExit()) { break; }
    if (seq.kb.getRecord()) {
        if (seq.isRecording) {
            for (0 => int i; i < 4; i++) {
                0 => recordings[i].record;
            }
            0 => seq.isRecording;
        }
        else {
            for (0 => int i; i < 4; i++) {
                1 => recordings[i].record;
            }
            1 => seq.isRecording;
        }
    }
}

for (0 => int i; i < 4; i++) {
    10::ms => now;
    0 => recordings[i].record;
    10::ms => now;
    recordings[i].closeFile();
}

for (0 => int i; i < 10; i++) {
    mainShreds[i].exit();
    10::ms => now;
}

rL.exit();
me.exit();
