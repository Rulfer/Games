/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 
 *       .____.                                                                 Thank you for buying my VRWeapons asset pack!
 *      xuu$``$$$uuu.
 *    . $``$  $$$`$$$                                           There are a couple things I couldn't automate, that you need to do to get started:
 *   dP*$  $  $$$ $$$
 *   ?k $  $  $$$ $$$                                           1. Ensure you have SteamVR in your current project.
 *    $ $  $  $$$ $$$                                           2. Add a layer called "Weapon", and 2 tags called "ReloadPoint" and "Magazine". *CASE SENSITIVE*
 *    ":$  $  $$$ $$$                                           3. Add the "InitializeControllers" prefab into your scene.
 *     N$  $  $$$ $$$                                           4. Drag the SteamVR controllers (left and right) onto the "Controllers" array of InitializeControllers.
 *     $$  $  $$$ $$$                                           5. Add my "Weapon.cs" script to the object you want to use as your weapon.
 *      $  $  $$$ $$$                                           6. In the inspector, you will find all necessary settings and a "Build weapon" button to walk you through the construction of your gun.
 *      $  $  $$$ $$$                                           
 *      $  $  $$$ $$$                                           If you run into any problems, check out this video I made to show how it works: https://www.youtube.com/watch?v=n-b_-6llmxM
 *      $  $  $$$ $$$                                           
 *      $  $  $$$ $$$                                           If you are still having issues, you can reach me at brad.chopper@gmail.com or on Twitter @Slayd7
 *      $$#$  $$$ $$$                                           
 *      $$'$  $$$ $$$                                           Thanks, and enjoy!
 *      $$`R  $$$ $$$
 *      $$$&  $$$ $$$
 *      $#*$  $$$ $$$
 *      $  $  $$$ @$$
 *      $  $  $$$ $$$
 *      $  $  $$$ $$$
 *      $  $  $B$ $$&.
 *      $  $  $D$ $$$$$muL.
 *      $  $  $Q$ $$$$$  `"**mu..
 *      $  $  $R$ $$$$$    k  `$$*t
 *      $  @  $$$ $$$$$    k   $$!4
 *      $ x$uu@B8u$NB@$uuuu6...$$X?
 *      $ $(`RF`$`````R$ $$5`"""#"R
 *      $ $" M$ $     $$ $$$      ?
 *      $ $  ?$ $     T$ $$$      $
 *      $ $F H$ $     M$ $$K      $  ..
 *      $ $L $$ $     $$ $$R.     "d$$$$Ns.
 *      $ $~ $$ $     N$ $$X      ."    "%2h
 *      $ 4k f  $     *$ $$&      R       "iN
 *      $ $$ %uz!     tuuR$$:     Buu      ?`:
 *      $ $F          $??$8B      | '*Ned*$~L$
 *      $ $k          $'@$$$      |$.suu+!' !$
 *      $ ?N          $'$$@$      $*`      d:"
 *      $ dL..........M.$&$$      5       d"P
 *    ..$.^"*I$RR*$C""??77*?      "nu...n*L*
 *   '$C"R   ``""!$*@#""` .uor    bu8BUU+!`
 *   '*@m@.       *d"     *$Rouxxd"```$
 *        R*@mu.           "#$R *$    !
 *        *%x. "*L               $     %.
 *           "N  `%.      ...u.d!` ..ue$$$o..
 *            @    ".    $*"""" .u$$$$$$$$$$$$beu...
 *           8  .mL %  :R`     x$$$$$$$$$$$$$$$$$$$$$$$$$$WmeemeeWc
 *          |$e!" "s:k 4      d$N"`"#$$$$$$$$$$$$$$$$$$$$$$$$$$$$$>
 *          $$      "N @      $?$   <F$$$$$$$$$$$$$$$$$$$$$$$$$$$$>
 *          $@       ^%Uu..   R#8buu$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$>
 *                     ```""*u$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$>
 *                            #$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$>
 *                             "5$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$>
 *                               `*$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$>
 *                                 ^#$$$$$$$$$$$$$$$$$$$$$$$$$$$$$>
 *                                    "*$$$$$$$$$$$$$$$$$$$$$$$$$$>
 *                                      `"*$$$$$$$$$$$$$$$$$$$$$$$>
 *                                          ^!$$$$$$$$$$$$$$$$$$$$>
 *                                              `"#+$$$$$$$$$$$$$$>
 *                                                    ""**$$$$$$$$>
 *                                                           ```""
 *
 * 
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */