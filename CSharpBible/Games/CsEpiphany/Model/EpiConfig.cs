using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsEpiphany.Model.Interfaces;

namespace CsEpiphany.Model;

// Singleton class to store game configuration
public class EpiConfig : IEpiConfig
{
    private int m_screen_size_x=640;
    private int m_screen_size_y=480;
    private int m_score_size_y=32;
    //	int m_sprite_size;
    private int m_map_size_x=32;
    private int m_map_size_y=32;
    static private int m_max_anim_drawn=8;
    //	int m_max_anim;
    private int m_moving_step= Sprite.size / m_max_anim_drawn;
    private int m_msec_per_frame= 140 / m_max_anim_drawn;
    private int m_volume_sound=8;
    private int m_volume_music=8;
    private int m_last_level=0;

    public void set_default_values() {
        //size in pixels
        m_screen_size_x = 640;
        m_screen_size_y = 480;
        m_score_size_y = 32;


        //size in cells
        m_map_size_x = 32;
        m_map_size_y = 32;

        //it must be a multiple of 2
        m_max_anim_drawn = 8;



        m_moving_step = Sprite.size / m_max_anim_drawn;

        //m_msec_per_frame=176/m_max_anim_drawn;
        //m_msec_per_frame=150/m_max_anim_drawn;
        //m_msec_per_frame=240/m_max_anim_drawn;
        m_msec_per_frame = 140 / m_max_anim_drawn;

        m_volume_sound = 8;

        m_volume_music = 8;

        m_last_level = 0;
    }
    public int get_screen_size_x()=> m_screen_size_x;
    public int get_screen_size_y()=> m_screen_size_y;
    public int get_score_size_y()=> m_score_size_y;

    //	int get_sprite_size();

    public int get_map_size_x()=> m_map_size_x;
    public int get_map_size_y()=> m_map_size_y;
    public int get_max_anim_drawn()=> m_max_anim_drawn;

    //	int get_max_anim();
    public int get_moving_step()=> m_moving_step;
    public int get_msec_per_frame()=> m_msec_per_frame;
    public int get_volume_sound();
    public int get_volume_music();
    public int get_last_level();
    public void set_last_level(int level);
    public void set_volume_sound(int volume);
    public void set_volume_music(int volume);
    public void read_values_from_file(char* filename);
    public void save_values_to_file(char* filename);
    public void refresh_game_window_parameters();

    // begin Singleton stuff

    private static EpiConfig? _instance;

    protected EpiConfig() {
        refresh_game_window_parameters();
    }

    public static EpiConfig Instance=> _instance ??= new EpiConfig();


}
