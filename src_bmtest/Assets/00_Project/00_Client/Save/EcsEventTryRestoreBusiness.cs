namespace Client {

    /// <summary>
    /// После запуска игры будут созданы сущности бизнеса,
    /// поле создания с данными из конфига будет попытка восстановить
    /// УЧЕСТЬ перезаписываемые поля, например изменение Имени бизнеса восстановится старое,даже после изменения в конфиге
    /// вручную проверить что перезаписываем
    /// данные из сохранения
    /// </summary>
    struct EcsEventTryRestoreBusiness {
        public int EntityBusiness;
        public int IDconfig;
    }
}