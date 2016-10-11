SpatialAudioSource
------------------

This package provides a realtime audio filter which simulates surround sound
using normal stereo headphones.

The filter works by augmenting the existing AudioSource component. Just drag
and drop the SpatialAudioSource component onto an existing audio source. 

Make sure to add the SpatialAudioListener onto the same GameObject as your
existing AudioListener. The default values for this component are likely to be
suitable for most games, only subtle adjustments should need to be made. All
parameters have tool tips which explain their usage.

How does it work?
It uses phase shifting to augment the builtin 3D panning and fading. This adds
much more horizontal directionality to the audio. Vertical directionality 
(front/back) is simulated by mixing in a small reverb signal and damping 
certain frequencies, which gives the player subtle cues when the sound is 
behind them. 

