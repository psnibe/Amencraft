16 => int numClips;
SndBuf2 inputClips[numClips];
WvOut2 outputClips[numClips];

SndBuf2 reference;
int refSamples;
float preSyncSamples;
float sampleDif;
float ratio;

44100 => float sampleRate;
8 => float beats;
169.9927 => float refBPM; //hardcoded; change if different ref clip
float preSyncBPM;
float BPMdif;
float BPMpercentChange;

me.dir()+"/ref/0.wav" => reference.read;
reference.samples() => refSamples;

for (0 => int i; i < numClips; i++)
{
    me.dir()+"/loops/"+i+".wav" => inputClips[i].read;
    inputClips[i] => outputClips[i] => blackhole;
    
    inputClips[i].samples() => preSyncSamples;
    refSamples - preSyncSamples => sampleDif;
    
    if (sampleDif > 0) {
        sampleDif / preSyncSamples => ratio;
        1.0 - ratio => inputClips[i].rate;
    }
    else if (sampleDif < 0) {
        Math.fabs(sampleDif) => sampleDif;
        sampleDif / preSyncSamples => ratio;
        1.0 + ratio => inputClips[i].rate;
    }
    
    60 / (preSyncSamples / (sampleRate * beats)) => preSyncBPM;
    preSyncBPM - refBPM => BPMdif;
    (BPMdif / preSyncBPM) * 100 => BPMpercentChange;
    if (BPMdif < 0) { -BPMpercentChange => BPMpercentChange; }
    
    chout <= refBPM + " / " + BPMdif + " / " + preSyncBPM + " / " + BPMpercentChange + IO.newline();
    
    // Uncomment to output adjusted clips
    //"Track_" + (i+1) => outputClips[i].autoPrefix;
    //"special:auto" => outputClips[i].wavFilename;
    //1 => outputClips[i].record;
    
    //refSamples::samp => now;
    
    //0 => outputClips[i].record;
    //outputClips[i].closeFile();
}
