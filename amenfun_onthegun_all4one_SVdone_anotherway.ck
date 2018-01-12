class Keyboard {
    
    KBHit kb;
    0 => int reboot;
    0 => int prev;
    0 => int next;
    0 => int back;
    0 => int rand;
    0 => int swap;
    [0,0,0,0] @=> int reverse[];
    0 => int switchOp;
    0 => int revMode;
    0 => int canWriteSelect;
    0 => int canWritePosition;
    0 => int downshift;
    0 => int upshift;
    0 => int sweep;
    0 => int sweepDir;
    0 => int variance;
    0 => int varianceDir;
    int lastSelectVal;
    int lastPositionVal;
    int tempVal;
    
    fun void listen()
    {
        while( true )
        {
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
            // pressing/holding "r" reverses playback for sample player 1
            if (tempVal == 114) { 
                if (revMode) {
                    if (reverse[0]) { 0 => reverse[0]; }
                    else { 1 => reverse[0]; }
                }
                else { 1 => reverse[0]; }
            }
            // pressing/holding "t" reverses playback for sample player 2
            if (tempVal == 116) { 
                if (revMode) {
                    if (reverse[1]) { 0 => reverse[1]; }
                    else { 1 => reverse[1]; }
                }
                else { 1 => reverse[1]; }
            }
            // pressing/holding "y" reverses playback for sample player 3
            if (tempVal == 121) { 
                if (revMode) {
                    if (reverse[2]) { 0 => reverse[2]; }
                    else { 1 => reverse[2]; }
                }
                else { 1 => reverse[2]; }
            }
            // pressing/holding "u" reverses playback for sample player 4
            if (tempVal == 117) { 
                if (revMode) {
                    if (reverse[3]) { 0 => reverse[3]; }
                    else { 1 => reverse[3]; }
                }
                else { 1 => reverse[3]; }
            }
            // pressing "q" toggles sequencer's step determination mode
            if (tempVal == 113) { 1 => switchOp; }
            // pressing "w" toggles auto-hold for reverse playback
            if (tempVal == 119) { 
                if (revMode) { 0 => revMode; }
                else { 1 => revMode; }
            }
            // pressing "-" writes last pressed position value in step
            if (tempVal == 45) { 1 => canWritePosition; }
            // pressing "=" writes last pressed select value in step
            if (tempVal == 61) { 1 => canWriteSelect; }
            // pressing numbers 1 - 8 sets lastPositionVal up to write
            if ((tempVal > 48) && (tempVal < 57)) { 
                (tempVal - 49) => lastPositionVal; 
            }
            // pressing letters a - p sets lastSelectVal up to write
            if ((tempVal > 96) && (tempVal < 113)) { 
                    (tempVal - 97) => lastSelectVal;
            }
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
    
    fun int getSwitch () 
    {
        if (switchOp) { 
            0 => switchOp;
            return 1; 
        }
        else { return 0; }
    }
    
    fun int getReverse (int which) 
    {
        if (revMode) {
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
    
    fun int getLastSelect () 
    {
        if (canWriteSelect) { 
            0 => canWriteSelect;
            return lastSelectVal; 
        }
        else { return -1; }
    }
    
    fun int getLastPosition () 
    {
        if (canWritePosition) { 
            0 => canWritePosition;
            return lastPositionVal; 
        }
        else { return -1; }
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
    
}

class Sequencer {
    
    Keyboard kb;
    
    // select seed preset list (slot 0 stores last played sequence)
    [ [15,1,9,9,1,5,10,5],
      [15,1,9,9,1,5,10,5],
      [0,4,4,4,4,9,4,4],
      [0,4,4,4,4,9,4,4],
      [2,1,12,9,14,5,10,5],
      [11,14,13,3,1,14,14,7],
      [7,8,14,13,7,7,2,7],
      [3,14,8,0,4,10,4,1],
      [10,12,10,0,1,12,3,10],
      [12,8,1,11,13,3,9,5],
      [2,2,1,13,2,15,2,7] ] @=> int selectPack[][];
      
    // position seed preset list (remaining slots are for playlist)
    [ [0,1,2,0,6,7,3,7],
      [0,1,2,0,6,7,3,7],
      [3,1,4,1,7,2,4,2],
      [3,1,4,1,5,9,2,6],
      [6,2,3,6,1,6,1,1],
      [1,5,3,5,0,7],
      [1,5,1,4,5,7],
      [2,2,5,4,1],
      [5,0,6,0,3],
      [3,1,2,5,5],
      [7,3,4,4,5] ] @=> int positionPack[][];
    
    // initial seeds, can be altered to taste, capacity determines step #
    selectPack[0] @=> int select[];
    positionPack[0] @=> int position[];
    // ^--(they get overwritten in real time using the step-insert keys)
    
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
        chout <= IO.newline();
    }
    
    fun void printPositionArray() 
    {
        chout <= "| ";
        for (0 => int i; i < position.cap(); i++) {
            chout <= position[i] <= " | ";
        }
        chout <= IO.newline();
    }
       
}

16 => int channels;
SndBuf amen[channels];
LPF lowpass[channels];
HPF hipass[channels];
JCRev rev1[channels];
NRev rev2[channels];
Pan2 pan[channels];
0 => int allsamples;
Sequencer seq;

for (0 => int i; i < channels; i++)
{
    amen[i] => rev1[i];
    // BYOL(bring your own loops), wants 16 of them labeled 0.wav - 15.wav
    me.dir()+"/audiobin/amen/loops/"+i+".wav" => amen[i].read;
    // ^-- (all samples must have the same length and tempo)
    amen[i].samples() +=> allsamples;
    amen[i].samples() => amen[i].pos;

    rev1[i] => rev2[i] => lowpass[i] => hipass[i] => pan[i] => dac;
    16000.0 => lowpass[i].freq;
    20.0 => hipass[i].freq;
    2.0 => lowpass[i].Q;
    2.0 => hipass[i].Q;
    0.0 => rev1[i].mix;
    0.0 => rev2[i].mix;
}

16 /=> allsamples;
allsamples/2 => int bar;
bar/4 => int beat;

Shred mainShreds[7];
0 => int shredCount;
0 => int flush;

// modifications to rate require inverse ratio as multiplier to tempo
[0.8,1.0,1.25] @=> float rate[];
[1.25,1.0,0.8] @=> float tempoMod[];
0 => int ratedex;

[[0,0],[0,0],[0,0],[0,0]] @=> int playerInfo[][];

fun void playAmen(int id, float gain, float rate, float tempo) 
{
    tempo $ int => playerInfo[id][1];
    while (true)
    {
        int selector;
        if (seq.opMode) {
            seq.getNextSelect(id) => selector;
            beat*(seq.getNextPosition(id)) => amen[selector].pos;
        }
        else {
            seq.getNextSelect() => selector;
            beat*(seq.getNextPosition()) => amen[selector].pos;
        }
        selector => playerInfo[id][0];
        gain => amen[selector].gain;
        if (seq.kb.getReverse(id)) { -rate => amen[selector].rate; }
        else { rate => amen[selector].rate; }
        Math.random2f(-0.75,0.75) => pan[selector].pan;
        Math.random2f(0.00,0.05) => rev1[selector].mix;
        Math.random2f(0.00,0.05) => rev2[selector].mix;
        
        seq.kb.getVariance() => int varDir;
        if (varDir >= 0) {
            now + (tempo/2)::samp => time later1;
            now + tempo::samp => time later2;            
            rate => float rateVariance;
            -0.0001 => float varInc;
            if (varDir) { 0.0001 => varInc; }
            varInc * (id + 1) => varInc;                      
            if (varDir >= 0) {
                while (now < later1) {
                    varInc +=> rateVariance;
                    rateVariance => amen[selector].rate;
                    1::samp => now;
                }
                while (now < later2) {
                    varInc -=> rateVariance;
                    rateVariance => amen[selector].rate;
                    1::samp => now;
                }
            }
        }
        else { tempo::samp => now; }
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
        (tempoMod[ratedex]*beat)::samp => now;
        if (seq.kb.getPrevious()) { seq.scrollLeft(); }
        if (seq.kb.getNext()) { seq.scrollRight(); }
        if (seq.kb.getBack()) { seq.goBack(); }
        if (seq.kb.swapIn()) { seq.swapInPlace(); }
        
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
        (tempoMod[ratedex]*beat)::samp => now;
        if (seq.kb.getRandom()) { seq.randomize(); }
        
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
    
    7 => shredCount;
    0 => flush;
}

rollOut();

fun void resetListener() {
    while( true ) {
        (tempoMod[ratedex]*beat)::samp => now;
        if (shredCount < 7) {
            mainShreds[0].exit();
            mainShreds[1].exit();
            mainShreds[2].exit();
            mainShreds[3].exit();
            mainShreds[4].exit();
            mainShreds[5].exit();
            mainShreds[6].exit();
            rollOut();
        }   
    }
}

spork ~ resetListener();

fun void speedListener() {
    while( true ) {
        (tempoMod[ratedex]*beat)::samp => now;
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
    }
}

spork ~ speedListener();

fun void switchListener() {
    while( true ) {
        (tempoMod[ratedex]*(beat/2))::samp => now;
        if (seq.kb.getSwitch()) {
            if (seq.opMode) { 0 => seq.opMode; }
            else { 1 => seq.opMode; }
        }
    }
}

spork ~ switchListener();

fun void sweepListener(int id, float lpInc, float hpInc) {
    16000.0 => float lpFreq;
    20.0 => float hpFreq;
    while( true ) {
        seq.kb.getSweep() => int sweepDir;
        if (sweepDir >= 0) {            
            now + playerInfo[id][1]::samp => time later;
            while (now < later) {
                if (sweepDir) {
                    hpInc +=> hpFreq;
                    if (hpFreq < 16000.0) {
                        hpFreq => hipass[(playerInfo[id][0])].freq;
                    }
                }
                else {
                    lpInc -=> lpFreq;
                    if (lpFreq > 20.0) {
                        lpFreq => lowpass[(playerInfo[id][0])].freq;
                    }
                }
                1::samp => now;
            }
            16000.0 => lpFreq;
            20.0 => hpFreq;
            lpFreq => lowpass[(playerInfo[id][0])].freq;
            hpFreq => hipass[(playerInfo[id][0])].freq;
        }
        else { (tempoMod[ratedex]*beat)::samp => now; }
    }
}

spork ~ sweepListener(0, 0.45, 0.005);
spork ~ sweepListener(1, 0.7, 0.005);
spork ~ sweepListener(2, 1.1, 0.005);
spork ~ sweepListener(3, 1.45, 0.005);

while( true ) // keep the fire alive
    1::second => now;