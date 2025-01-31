using CsEpiphany.Model.Interfaces;

namespace CsEpiphany.Model;

// Singleton class to store game configuration
public class EpiConfig : IEpiConfig
{
    private int _screen_size_x=640;
    private int _screen_size_y=480;
    private int _score_size_y=32;
    //	int m_sprite_size;
    private int _map_size_x=32;
    private int _map_size_y=32;
    static private int _max_anim_drawn=8;
    //	int m_max_anim;
    private int _moving_step= Sprite.size / _max_anim_drawn;
    private int _msec_per_frame= 140 / _max_anim_drawn;
    private int _volume_sound=8;
    private int _volume_music=8;
    private int m_last_level=0;

    public void set_default_values() {
        //size in pixels
        _screen_size_x = 640;
        _screen_size_y = 480;
        _score_size_y = 32;


        //size in cells
        _map_size_x = 32;
        _map_size_y = 32;

        //it must be a multiple of 2
        _max_anim_drawn = 8;



        _moving_step = Sprite.size / _max_anim_drawn;

        //_msec_per_frame=176/_max_anim_drawn;
        //_msec_per_frame=150/_max_anim_drawn;
        //_msec_per_frame=240/_max_anim_drawn;
        _msec_per_frame = 140 / _max_anim_drawn;

        _volume_sound = 8;

        _volume_music = 8;

        m_last_level = 0;
    }
    public int get_screen_size_x()=> _screen_size_x;
    public int get_screen_size_y()=> _screen_size_y;
    public int get_score_size_y()=> _score_size_y;

    //	int get_sprite_size();

    public int get_map_size_x()=> _map_size_x;
    public int get_map_size_y()=> _map_size_y;
    public int get_max_anim_drawn()=> _max_anim_drawn;

    //	int get_max_anim();
    public int get_moving_step()=> _moving_step;
    public int get_msec_per_frame()=> _msec_per_frame;
    public int get_volume_sound()=>_volume_sound;
    public int get_volume_music()=> _volume_music;
    public int get_last_level()=>m_last_level;
    public void set_last_level(int level)=>m_last_level=level;
    public void set_volume_sound(int volume)=>_volume_sound=volume;
    public void set_volume_music(int volume)=>_volume_music=volume;
    public void read_values_from_file(string filename) { }
    public void save_values_to_file(string filename) { }
    public void refresh_game_window_parameters() { }

    // begin Singleton stuff

    private static EpiConfig? _instance;

    protected EpiConfig() {
        refresh_game_window_parameters();
    }

    public static EpiConfig Instance=> _instance ??= new EpiConfig();


}
