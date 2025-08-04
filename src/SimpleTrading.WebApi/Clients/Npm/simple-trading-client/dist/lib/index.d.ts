export interface ISimpleTradingClient {
    /**
     * @return OK
     */
    getAppInfo(signal?: AbortSignal): Promise<SimpleTradingClientResponse<ApiInfo>>;

    /**
     * @return OK
     */
    getUserSettings(signal?: AbortSignal): Promise<SimpleTradingClientResponse<UserSettingsDto>>;

    /**
     * @return No Content
     */
    updateUserSettings(body: UpdateUserSettingsDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;

    /**
     * @return OK
     */
    getUserLocalNow(signal?: AbortSignal): Promise<SimpleTradingClientResponse<Date>>;

    /**
     * @return OK
     */
    getAvailableTimezones(signal?: AbortSignal): Promise<SimpleTradingClientResponse<TimeZoneOption[]>>;

    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getAssets(searchTerm: string | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<AssetDto[]>>;

    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getCurrencies(searchTerm: string | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<CurrencyDto[]>>;

    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getProfiles(searchTerm: string | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<ProfileDto[]>>;

    /**
     * @return OK
     */
    getActiveProfile(signal?: AbortSignal): Promise<SimpleTradingClientResponse<ProfileDto>>;

    /**
     * @return OK
     */
    getReference(tradeId: string, referenceId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<ReferenceDto>>;

    /**
     * @return No Content
     */
    updateReference(tradeId: string, referenceId: string, body: UpdateReferenceDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;

    /**
     * @return No Content
     */
    deleteReference(tradeId: string, referenceId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;

    /**
     * @return OK
     */
    getReferences(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<ReferenceDto[]>>;

    /**
     * @return OK
     */
    addReference(tradeId: string, body: AddReferenceDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<string>>;

    /**
     * @return OK
     */
    deleteReferences(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<number>>;

    /**
     * @param profileId (optional)
     * @param sort (optional)
     * @param filter (optional)
     * @param page (optional)
     * @param pageSize (optional)
     * @return OK
     */
    searchTrades(profileId: string | undefined, sort: string[] | undefined, filter: string[] | undefined, page: number | undefined, pageSize: number | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<PageDtoOfTradeDto>>;

    /**
     * @return OK
     */
    addTrade(body: AddTradeDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<AddTradeResultDto>>;

    /**
     * @return OK
     */
    getTrade(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<TradeDto>>;

    /**
     * @return OK
     */
    updateTrade(tradeId: string, body: UpdateTradeDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<WarningsDto>>;

    /**
     * @return No Content
     */
    deleteTrade(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;

    /**
     * @return OK
     */
    closeTrade(tradeId: string, body: CloseTradeDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<TradeResultDto>>;

    /**
     * @return OK
     */
    restoreCalculatedResult(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<TradeResultDto>>;
}

export declare class SimpleTradingClient implements ISimpleTradingClient {
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined;
    private http;
    private baseUrl;

    constructor(baseUrl?: string, http?: {
        fetch(url: RequestInfo, init?: RequestInit): Promise<Response>;
    });

    /**
     * @return OK
     */
    getAppInfo(signal?: AbortSignal): Promise<SimpleTradingClientResponse<ApiInfo>>;

    /**
     * @return OK
     */
    getUserSettings(signal?: AbortSignal): Promise<SimpleTradingClientResponse<UserSettingsDto>>;

    /**
     * @return No Content
     */
    updateUserSettings(body: UpdateUserSettingsDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;

    /**
     * @return OK
     */
    getUserLocalNow(signal?: AbortSignal): Promise<SimpleTradingClientResponse<Date>>;

    /**
     * @return OK
     */
    getAvailableTimezones(signal?: AbortSignal): Promise<SimpleTradingClientResponse<TimeZoneOption[]>>;

    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getAssets(searchTerm: string | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<AssetDto[]>>;

    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getCurrencies(searchTerm: string | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<CurrencyDto[]>>;

    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getProfiles(searchTerm: string | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<ProfileDto[]>>;

    /**
     * @return OK
     */
    getActiveProfile(signal?: AbortSignal): Promise<SimpleTradingClientResponse<ProfileDto>>;

    /**
     * @return OK
     */
    getReference(tradeId: string, referenceId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<ReferenceDto>>;

    /**
     * @return No Content
     */
    updateReference(tradeId: string, referenceId: string, body: UpdateReferenceDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;

    /**
     * @return No Content
     */
    deleteReference(tradeId: string, referenceId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;

    /**
     * @return OK
     */
    getReferences(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<ReferenceDto[]>>;

    /**
     * @return OK
     */
    addReference(tradeId: string, body: AddReferenceDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<string>>;

    /**
     * @return OK
     */
    deleteReferences(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<number>>;

    /**
     * @param profileId (optional)
     * @param sort (optional)
     * @param filter (optional)
     * @param page (optional)
     * @param pageSize (optional)
     * @return OK
     */
    searchTrades(profileId: string | undefined, sort: string[] | undefined, filter: string[] | undefined, page: number | undefined, pageSize: number | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<PageDtoOfTradeDto>>;

    /**
     * @return OK
     */
    addTrade(body: AddTradeDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<AddTradeResultDto>>;

    /**
     * @return OK
     */
    getTrade(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<TradeDto>>;

    /**
     * @return OK
     */
    updateTrade(tradeId: string, body: UpdateTradeDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<WarningsDto>>;

    /**
     * @return No Content
     */
    deleteTrade(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;

    /**
     * @return OK
     */
    closeTrade(tradeId: string, body: CloseTradeDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<TradeResultDto>>;

    /**
     * @return OK
     */
    restoreCalculatedResult(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<TradeResultDto>>;

    protected processGetAppInfo(response: Response): Promise<SimpleTradingClientResponse<ApiInfo>>;

    protected processGetUserSettings(response: Response): Promise<SimpleTradingClientResponse<UserSettingsDto>>;

    protected processUpdateUserSettings(response: Response): Promise<SimpleTradingClientResponse<void>>;

    protected processGetUserLocalNow(response: Response): Promise<SimpleTradingClientResponse<Date>>;

    protected processGetAvailableTimezones(response: Response): Promise<SimpleTradingClientResponse<TimeZoneOption[]>>;

    protected processGetAssets(response: Response): Promise<SimpleTradingClientResponse<AssetDto[]>>;

    protected processGetCurrencies(response: Response): Promise<SimpleTradingClientResponse<CurrencyDto[]>>;

    protected processGetProfiles(response: Response): Promise<SimpleTradingClientResponse<ProfileDto[]>>;

    protected processGetActiveProfile(response: Response): Promise<SimpleTradingClientResponse<ProfileDto>>;

    protected processGetReference(response: Response): Promise<SimpleTradingClientResponse<ReferenceDto>>;

    protected processUpdateReference(response: Response): Promise<SimpleTradingClientResponse<void>>;

    protected processDeleteReference(response: Response): Promise<SimpleTradingClientResponse<void>>;

    protected processGetReferences(response: Response): Promise<SimpleTradingClientResponse<ReferenceDto[]>>;

    protected processAddReference(response: Response): Promise<SimpleTradingClientResponse<string>>;

    protected processDeleteReferences(response: Response): Promise<SimpleTradingClientResponse<number>>;

    protected processSearchTrades(response: Response): Promise<SimpleTradingClientResponse<PageDtoOfTradeDto>>;

    protected processAddTrade(response: Response): Promise<SimpleTradingClientResponse<AddTradeResultDto>>;

    protected processGetTrade(response: Response): Promise<SimpleTradingClientResponse<TradeDto>>;

    protected processUpdateTrade(response: Response): Promise<SimpleTradingClientResponse<WarningsDto>>;

    protected processDeleteTrade(response: Response): Promise<SimpleTradingClientResponse<void>>;

    protected processCloseTrade(response: Response): Promise<SimpleTradingClientResponse<TradeResultDto>>;

    protected processRestoreCalculatedResult(response: Response): Promise<SimpleTradingClientResponse<TradeResultDto>>;
}

export declare class AddReferenceDto implements IAddReferenceDto {
    type?: NullableOfReferenceTypeDto | undefined;
    link?: string | undefined;
    notes?: string | undefined;

    [key: string]: any;

    constructor(data?: IAddReferenceDto);

    static fromJS(data: any): AddReferenceDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IAddReferenceDto {
    type?: NullableOfReferenceTypeDto | undefined;
    link?: string | undefined;
    notes?: string | undefined;

    [key: string]: any;
}

export declare class AddTradeDto implements IAddTradeDto {
    dryRun?: boolean | undefined;
    assetId?: string | undefined;
    profileId?: string | undefined;
    opened?: Date | undefined;
    closed?: Date | undefined;
    size?: number | undefined;
    manuallyEnteredResult?: UpdateResultValue | undefined;
    profitLoss?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: number | undefined;
    takeProfit?: number | undefined;
    exitPrice?: number | undefined;
    notes?: string | undefined;
    references?: AddReferenceDto[] | undefined;

    [key: string]: any;

    constructor(data?: IAddTradeDto);

    static fromJS(data: any): AddTradeDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IAddTradeDto {
    dryRun?: boolean | undefined;
    assetId?: string | undefined;
    profileId?: string | undefined;
    opened?: Date | undefined;
    closed?: Date | undefined;
    size?: number | undefined;
    manuallyEnteredResult?: UpdateResultValue | undefined;
    profitLoss?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: number | undefined;
    takeProfit?: number | undefined;
    exitPrice?: number | undefined;
    notes?: string | undefined;
    references?: AddReferenceDto[] | undefined;

    [key: string]: any;
}

export declare class AddTradeResultDto implements IAddTradeResultDto {
    tradeId: string;
    warnings: string[];

    [key: string]: any;

    constructor(data?: IAddTradeResultDto);

    static fromJS(data: any): AddTradeResultDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IAddTradeResultDto {
    tradeId: string;
    warnings: string[];

    [key: string]: any;
}

export declare class ApiInfo implements IApiInfo {
    name: string;
    version: string;
    environment: string;

    [key: string]: any;

    constructor(data?: IApiInfo);

    static fromJS(data: any): ApiInfo;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IApiInfo {
    name: string;
    version: string;
    environment: string;

    [key: string]: any;
}

export declare class AssetDto implements IAssetDto {
    id: string;
    symbol: string;
    name: string;

    [key: string]: any;

    constructor(data?: IAssetDto);

    static fromJS(data: any): AssetDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IAssetDto {
    id: string;
    symbol: string;
    name: string;

    [key: string]: any;
}

export declare class CloseTradeDto implements ICloseTradeDto {
    profitLoss?: number | undefined;
    exitPrice?: number | undefined;
    closed?: Date | undefined;
    manuallyEnteredResult?: UpdateResultValue | undefined;

    [key: string]: any;

    constructor(data?: ICloseTradeDto);

    static fromJS(data: any): CloseTradeDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface ICloseTradeDto {
    profitLoss?: number | undefined;
    exitPrice?: number | undefined;
    closed?: Date | undefined;
    manuallyEnteredResult?: UpdateResultValue | undefined;

    [key: string]: any;
}

export declare class CurrencyDto implements ICurrencyDto {
    id: string;
    isoCode: string;
    name: string;

    [key: string]: any;

    constructor(data?: ICurrencyDto);

    static fromJS(data: any): CurrencyDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface ICurrencyDto {
    id: string;
    isoCode: string;
    name: string;

    [key: string]: any;
}

export declare class ErrorResponse implements IErrorResponse {
    messages: string[];

    [key: string]: any;

    constructor(data?: IErrorResponse);

    static fromJS(data: any): ErrorResponse;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IErrorResponse {
    messages: string[];

    [key: string]: any;
}

export declare class FieldError implements IFieldError {
    identifier: string;
    messages: string[];

    [key: string]: any;

    constructor(data?: IFieldError);

    static fromJS(data: any): FieldError;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IFieldError {
    identifier: string;
    messages: string[];

    [key: string]: any;
}

export declare class FieldErrorResponse implements IFieldErrorResponse {
    errors: FieldError[];

    [key: string]: any;

    constructor(data?: IFieldErrorResponse);

    static fromJS(data: any): FieldErrorResponse;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IFieldErrorResponse {
    errors: FieldError[];

    [key: string]: any;
}

export type NullableOfReferenceTypeDto = "TradingView" | "Other";
export type NullableOfResultDto = "Win" | "Mediocre" | "BreakEven" | "Loss";

export declare class PageDtoOfTradeDto implements IPageDtoOfTradeDto {
    data: TradeDto[];
    count: number;
    totalCount: number;
    totalPages: number;
    page: number;
    pageSize: number;

    [key: string]: any;

    constructor(data?: IPageDtoOfTradeDto);

    static fromJS(data: any): PageDtoOfTradeDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IPageDtoOfTradeDto {
    data: TradeDto[];
    count: number;
    totalCount: number;
    totalPages: number;
    page: number;
    pageSize: number;

    [key: string]: any;
}

export declare class ProfileDto implements IProfileDto {
    id: string;
    name: string;
    description?: string | undefined;
    isActive: boolean;

    [key: string]: any;

    constructor(data?: IProfileDto);

    static fromJS(data: any): ProfileDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IProfileDto {
    id: string;
    name: string;
    description?: string | undefined;
    isActive: boolean;

    [key: string]: any;
}

export declare class ReferenceDto implements IReferenceDto {
    id: string;
    type: ReferenceTypeDto;
    link: string;
    notes?: string | undefined;

    [key: string]: any;

    constructor(data?: IReferenceDto);

    static fromJS(data: any): ReferenceDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IReferenceDto {
    id: string;
    type: ReferenceTypeDto;
    link: string;
    notes?: string | undefined;

    [key: string]: any;
}

export type ReferenceTypeDto = "TradingView" | "Other";

export declare class TimeZoneOption implements ITimeZoneOption {
    windowsId: string;
    timeZone: string;
    offset: string;

    [key: string]: any;

    constructor(data?: ITimeZoneOption);

    static fromJS(data: any): TimeZoneOption;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface ITimeZoneOption {
    windowsId: string;
    timeZone: string;
    offset: string;

    [key: string]: any;
}

export declare class TradeDto implements ITradeDto {
    id?: string;
    assetId?: string;
    asset: string;
    profileId?: string;
    profile: string;
    size?: number;
    opened?: Date;
    closed?: Date | undefined;
    profitLoss?: number | undefined;
    result?: NullableOfResultDto | undefined;
    performance?: number | undefined;
    isClosed?: boolean;
    currencyId?: string;
    currency: string;
    entry?: number;
    stopLoss?: number | undefined;
    takeProfit?: number | undefined;
    exitPrice?: number | undefined;
    riskRewardRatio?: number | undefined;
    references: ReferenceDto[];
    notes?: string | undefined;
    warnings: string[];

    [key: string]: any;

    constructor(data?: ITradeDto);

    static fromJS(data: any): TradeDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface ITradeDto {
    id?: string;
    assetId?: string;
    asset: string;
    profileId?: string;
    profile: string;
    size?: number;
    opened?: Date;
    closed?: Date | undefined;
    profitLoss?: number | undefined;
    result?: NullableOfResultDto | undefined;
    performance?: number | undefined;
    isClosed?: boolean;
    currencyId?: string;
    currency: string;
    entry?: number;
    stopLoss?: number | undefined;
    takeProfit?: number | undefined;
    exitPrice?: number | undefined;
    riskRewardRatio?: number | undefined;
    references: ReferenceDto[];
    notes?: string | undefined;
    warnings: string[];

    [key: string]: any;
}

export declare class TradeResultDto implements ITradeResultDto {
    tradeId: string;
    result: NullableOfResultDto | undefined;
    performance: number | undefined;
    warnings: string[];

    [key: string]: any;

    constructor(data?: ITradeResultDto);

    static fromJS(data: any): TradeResultDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface ITradeResultDto {
    tradeId: string;
    result: NullableOfResultDto | undefined;
    performance: number | undefined;
    warnings: string[];

    [key: string]: any;
}

export declare class UpdateReferenceDto implements IUpdateReferenceDto {
    type?: NullableOfReferenceTypeDto | undefined;
    link?: string | undefined;
    notes?: UpdateStringValue | undefined;

    [key: string]: any;

    constructor(data?: IUpdateReferenceDto);

    static fromJS(data: any): UpdateReferenceDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IUpdateReferenceDto {
    type?: NullableOfReferenceTypeDto | undefined;
    link?: string | undefined;
    notes?: UpdateStringValue | undefined;

    [key: string]: any;
}

export declare class UpdateResultValue implements IUpdateResultValue {
    value?: NullableOfResultDto | undefined;

    [key: string]: any;

    constructor(data?: IUpdateResultValue);

    static fromJS(data: any): UpdateResultValue;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IUpdateResultValue {
    value?: NullableOfResultDto | undefined;

    [key: string]: any;
}

export declare class UpdateStringValue implements IUpdateStringValue {
    value?: string | undefined;

    [key: string]: any;

    constructor(data?: IUpdateStringValue);

    static fromJS(data: any): UpdateStringValue;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IUpdateStringValue {
    value?: string | undefined;

    [key: string]: any;
}

export declare class UpdateTradeDto implements IUpdateTradeDto {
    assetId?: string | undefined;
    profileId?: string | undefined;
    opened?: Date | undefined;
    closed?: Date | undefined;
    size?: number | undefined;
    manuallyEnteredResult?: UpdateResultValue | undefined;
    profitLoss?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: UpdateValueOfNullableOfdecimal | undefined;
    takeProfit?: UpdateValueOfNullableOfdecimal | undefined;
    exitPrice?: UpdateValueOfNullableOfdecimal | undefined;
    notes?: UpdateStringValue | undefined;

    [key: string]: any;

    constructor(data?: IUpdateTradeDto);

    static fromJS(data: any): UpdateTradeDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IUpdateTradeDto {
    assetId?: string | undefined;
    profileId?: string | undefined;
    opened?: Date | undefined;
    closed?: Date | undefined;
    size?: number | undefined;
    manuallyEnteredResult?: UpdateResultValue | undefined;
    profitLoss?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: UpdateValueOfNullableOfdecimal | undefined;
    takeProfit?: UpdateValueOfNullableOfdecimal | undefined;
    exitPrice?: UpdateValueOfNullableOfdecimal | undefined;
    notes?: UpdateStringValue | undefined;

    [key: string]: any;
}

export declare class UpdateUserSettingsDto implements IUpdateUserSettingsDto {
    culture?: string | undefined;
    isoLanguageCode?: UpdateStringValue | undefined;
    timeZone?: string | undefined;

    [key: string]: any;

    constructor(data?: IUpdateUserSettingsDto);

    static fromJS(data: any): UpdateUserSettingsDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IUpdateUserSettingsDto {
    culture?: string | undefined;
    isoLanguageCode?: UpdateStringValue | undefined;
    timeZone?: string | undefined;

    [key: string]: any;
}

export declare class UpdateValueOfNullableOfdecimal implements IUpdateValueOfNullableOfdecimal {
    value?: number | undefined;

    [key: string]: any;

    constructor(data?: IUpdateValueOfNullableOfdecimal);

    static fromJS(data: any): UpdateValueOfNullableOfdecimal;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IUpdateValueOfNullableOfdecimal {
    value?: number | undefined;

    [key: string]: any;
}

export declare class UserSettingsDto implements IUserSettingsDto {
    culture: string;
    language: string | undefined;
    timeZone: string;
    lastModified: Date;

    [key: string]: any;

    constructor(data?: IUserSettingsDto);

    static fromJS(data: any): UserSettingsDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IUserSettingsDto {
    culture: string;
    language: string | undefined;
    timeZone: string;
    lastModified: Date;

    [key: string]: any;
}

export declare class WarningsDto implements IWarningsDto {
    warnings: string[];

    [key: string]: any;

    constructor(data?: IWarningsDto);

    static fromJS(data: any): WarningsDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IWarningsDto {
    warnings: string[];

    [key: string]: any;
}

export declare class SimpleTradingClientResponse<TResult> {
    status: number;
    headers: {
        [key: string]: any;
    };
    result: TResult;

    constructor(status: number, headers: {
        [key: string]: any;
    }, result: TResult);
}

export declare class SimpleTradingClientException extends Error {
    message: string;
    status: number;
    response: string;
    headers: {
        [key: string]: any;
    };
    result: any;
    protected isSimpleTradingClientException: boolean;

    constructor(message: string, status: number, response: string, headers: {
        [key: string]: any;
    }, result: any);

    static isSimpleTradingClientException(obj: any): obj is SimpleTradingClientException;
}
