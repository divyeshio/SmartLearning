namespace SmartLearning.Core.Constants;
public static class ApiRoutes
{
    public const string Root = "api";

    public const string Version = "v1";

    public const string Base = $"{Root}/{Version}/";


    public static class Boards
    {
        public const string GetAll = "/boards";
        public const string Add = "/board/add";
        public const string Update = "/board/update/";
        public const string Delete = "/board/delete";
    }

    public static class Class
    {
        public const string GetAll = "/class/all";
        public const string Add = "/class/add";
        public const string Update = "/class/update";
        public const string Delete = "/class/delete";
    }

    public static class Notes
    {
        public const string GetAll = "/notes/all";
        public const string Add = "/notes/add";
        public const string Update = "/notes/update";
        public const string Delete = "/notes/delete";
    }

}
