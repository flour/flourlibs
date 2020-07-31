using Flour.WebKitPDF.Commons;
using System;
using System.Runtime.InteropServices;

namespace Flour.WebKitPDF
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IntCallback(IntPtr converter, int integer);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void VoidCallback(IntPtr converter);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void StringCallback(IntPtr converter, [MarshalAs(UnmanagedType.LPStr)] string str);

    internal static class WebKitToHtmlExternals
    {
        public const string DllName = "libwkhtmltox";
        public const CharSet DefaultCharSet = CharSet.Unicode;

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_extended_qt();

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_version();

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_init(int useGraphics);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_deinit();

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_create_global_settings();

        [DllImport(DllName, CharSet = DefaultCharSet)]
        public static extern int wkhtmltopdf_set_global_setting(IntPtr settings,
            [MarshalAs((short)CustomUnmanagedType.LPUTF8Str)]
            string name,
            [MarshalAs((short)CustomUnmanagedType.LPUTF8Str)]
            string value);


        [DllImport(DllName, CharSet = DefaultCharSet)]
        public static extern int wkhtmltopdf_get_global_setting(IntPtr settings,
            [MarshalAs((short)CustomUnmanagedType.LPUTF8Str)]
            string name,
            IntPtr value, int valueSize);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_destroy_global_settings(IntPtr settings);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_create_object_settings();

        [DllImport(DllName, CharSet = DefaultCharSet)]
        public static extern int wkhtmltopdf_set_object_setting(IntPtr settings,
            [MarshalAs((short)CustomUnmanagedType.LPUTF8Str)]
            string name,
            [MarshalAs((short)CustomUnmanagedType.LPUTF8Str)]
            string value);

        [DllImport(DllName, CharSet = DefaultCharSet)]
        public static extern int wkhtmltopdf_get_object_setting(IntPtr settings,
            [MarshalAs((short)CustomUnmanagedType.LPUTF8Str)]
            string name,
            IntPtr value, int valueSize);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_destroy_object_settings(IntPtr settings);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_create_converter(IntPtr globalSettings);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkhtmltopdf_add_object(IntPtr converter,
            IntPtr objectSettings,
            byte[] data);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkhtmltopdf_add_object(IntPtr converter,
            IntPtr objectSettings,
            [MarshalAs((short)CustomUnmanagedType.LPUTF8Str)] string data);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool wkhtmltopdf_convert(IntPtr converter);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkhtmltopdf_destroy_converter(IntPtr converter);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_get_output(IntPtr converter, out IntPtr data);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_phase_changed_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] VoidCallback callback);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_progress_changed_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] VoidCallback callback);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_finished_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] IntCallback callback);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_warning_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] StringCallback callback);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_error_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] StringCallback callback);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_phase_count(IntPtr converter);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_current_phase(IntPtr converter);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_phase_description(IntPtr converter, int phase);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_progress_string(IntPtr converter);

        [DllImport(DllName, CharSet = DefaultCharSet, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_http_error_code(IntPtr converter);
    }
}
