export interface ISimpleTradingClient {
    /**
     * @return OK
     */
    getAppInfo(): Promise<SwaggerResponse<ApiInfo>>;

    /**
     * @return OK
     */
    getTrade(tradeId: string): Promise<SwaggerResponse<TradeDto>>;

    /**
     * @param body (optional)
     * @return OK
     */
    updateTrade(tradeId: string, body: UpdateTradeDto | undefined): Promise<SwaggerResponse<SuccessResponse>>;

    /**
     * @return OK
     */
    deleteTrade(tradeId: string): Promise<SwaggerResponse<SuccessResponse>>;

    /**
     * @param body (optional)
     * @return OK
     */
    addTrade(body: AddTradeDto | undefined): Promise<SwaggerResponse<GuidSuccessResponse>>;

    /**
     * @param body (optional)
     * @return OK
     */
    closeTrade(tradeId: string, body: CloseTradeDto | undefined): Promise<SwaggerResponse<SuccessResponse>>;

    /**
     * @return OK
     */
    getReference(tradeId: string, referenceId: string): Promise<SwaggerResponse<ReferenceDto>>;

    /**
     * @param body (optional)
     * @return OK
     */
    updateReference(tradeId: string, referenceId: string, body: UpdateReferenceDto | undefined): Promise<SwaggerResponse<SuccessResponse>>;

    /**
     * @return OK
     */
    deleteReference(tradeId: string, referenceId: string): Promise<SwaggerResponse<SuccessResponse>>;

    /**
     * @return OK
     */
    getReferences(tradeId: string): Promise<SwaggerResponse<ReferenceDto[]>>;

    /**
     * @param body (optional)
     * @return OK
     */
    addReference(tradeId: string, body: AddReferenceDto | undefined): Promise<SwaggerResponse<GuidSuccessResponse>>;

    /**
     * @return OK
     */
    deleteReferences(tradeId: string): Promise<SwaggerResponse<UInt16SuccessResponse>>;

    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getAssets(searchTerm: string | undefined): Promise<SwaggerResponse<AssetDto[]>>;

    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getProfiles(searchTerm: string | undefined): Promise<SwaggerResponse<ProfileDto[]>>;

    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getCurrencies(searchTerm: string | undefined): Promise<SwaggerResponse<CurrencyDto[]>>;
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
    getAppInfo(): Promise<SwaggerResponse<ApiInfo>>;

    /**
     * @return OK
     */
    getTrade(tradeId: string): Promise<SwaggerResponse<TradeDto>>;

    /**
     * @param body (optional)
     * @return OK
     */
    updateTrade(tradeId: string, body: UpdateTradeDto | undefined): Promise<SwaggerResponse<SuccessResponse>>;

    /**
     * @return OK
     */
    deleteTrade(tradeId: string): Promise<SwaggerResponse<SuccessResponse>>;

    /**
     * @param body (optional)
     * @return OK
     */
    addTrade(body: AddTradeDto | undefined): Promise<SwaggerResponse<GuidSuccessResponse>>;

    /**
     * @param body (optional)
     * @return OK
     */
    closeTrade(tradeId: string, body: CloseTradeDto | undefined): Promise<SwaggerResponse<SuccessResponse>>;

    /**
     * @return OK
     */
    getReference(tradeId: string, referenceId: string): Promise<SwaggerResponse<ReferenceDto>>;

    /**
     * @param body (optional)
     * @return OK
     */
    updateReference(tradeId: string, referenceId: string, body: UpdateReferenceDto | undefined): Promise<SwaggerResponse<SuccessResponse>>;

    /**
     * @return OK
     */
    deleteReference(tradeId: string, referenceId: string): Promise<SwaggerResponse<SuccessResponse>>;

    /**
     * @return OK
     */
    getReferences(tradeId: string): Promise<SwaggerResponse<ReferenceDto[]>>;

    /**
     * @param body (optional)
     * @return OK
     */
    addReference(tradeId: string, body: AddReferenceDto | undefined): Promise<SwaggerResponse<GuidSuccessResponse>>;

    /**
     * @return OK
     */
    deleteReferences(tradeId: string): Promise<SwaggerResponse<UInt16SuccessResponse>>;

    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getAssets(searchTerm: string | undefined): Promise<SwaggerResponse<AssetDto[]>>;

    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getProfiles(searchTerm: string | undefined): Promise<SwaggerResponse<ProfileDto[]>>;

    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getCurrencies(searchTerm: string | undefined): Promise<SwaggerResponse<CurrencyDto[]>>;

    protected processGetAppInfo(response: Response): Promise<SwaggerResponse<ApiInfo>>;

    protected processGetTrade(response: Response): Promise<SwaggerResponse<TradeDto>>;

    protected processUpdateTrade(response: Response): Promise<SwaggerResponse<SuccessResponse>>;

    protected processDeleteTrade(response: Response): Promise<SwaggerResponse<SuccessResponse>>;

    protected processAddTrade(response: Response): Promise<SwaggerResponse<GuidSuccessResponse>>;

    protected processCloseTrade(response: Response): Promise<SwaggerResponse<SuccessResponse>>;

    protected processGetReference(response: Response): Promise<SwaggerResponse<ReferenceDto>>;

    protected processUpdateReference(response: Response): Promise<SwaggerResponse<SuccessResponse>>;

    protected processDeleteReference(response: Response): Promise<SwaggerResponse<SuccessResponse>>;

    protected processGetReferences(response: Response): Promise<SwaggerResponse<ReferenceDto[]>>;

    protected processAddReference(response: Response): Promise<SwaggerResponse<GuidSuccessResponse>>;

    protected processDeleteReferences(response: Response): Promise<SwaggerResponse<UInt16SuccessResponse>>;

    protected processGetAssets(response: Response): Promise<SwaggerResponse<AssetDto[]>>;

    protected processGetProfiles(response: Response): Promise<SwaggerResponse<ProfileDto[]>>;

    protected processGetCurrencies(response: Response): Promise<SwaggerResponse<CurrencyDto[]>>;
}

export declare class AddReferenceDto implements IAddReferenceDto {
    type?: ReferenceTypeDto;
    link?: string | undefined;
    notes?: string | undefined;

    constructor(data?: IAddReferenceDto);

    static fromJS(data: any): AddReferenceDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IAddReferenceDto {
    type?: ReferenceTypeDto;
    link?: string | undefined;
    notes?: string | undefined;
}

export declare class AddTradeDto implements IAddTradeDto {
    assetId?: string | undefined;
    profileId?: string | undefined;
    opened?: Date | undefined;
    closed?: Date | undefined;
    size?: number | undefined;
    result?: ResultDto;
    balance?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: number | undefined;
    takeProfit?: number | undefined;
    exitPrice?: number | undefined;
    notes?: string | undefined;
    references?: AddReferenceDto[] | undefined;

    constructor(data?: IAddTradeDto);

    static fromJS(data: any): AddTradeDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IAddTradeDto {
    assetId?: string | undefined;
    profileId?: string | undefined;
    opened?: Date | undefined;
    closed?: Date | undefined;
    size?: number | undefined;
    result?: ResultDto;
    balance?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: number | undefined;
    takeProfit?: number | undefined;
    exitPrice?: number | undefined;
    notes?: string | undefined;
    references?: AddReferenceDto[] | undefined;
}

export declare class ApiInfo implements IApiInfo {
    name?: string | undefined;
    version?: string | undefined;
    environment?: string | undefined;

    constructor(data?: IApiInfo);

    static fromJS(data: any): ApiInfo;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IApiInfo {
    name?: string | undefined;
    version?: string | undefined;
    environment?: string | undefined;
}

export declare class AssetDto implements IAssetDto {
    id: string;
    symbol: string | undefined;
    name: string | undefined;

    constructor(data?: IAssetDto);

    static fromJS(data: any): AssetDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IAssetDto {
    id: string;
    symbol: string | undefined;
    name: string | undefined;
}

export declare class CloseTradeDto implements ICloseTradeDto {
    balance?: number | undefined;
    exitPrice?: number | undefined;
    closed?: Date | undefined;
    result?: ResultDto;

    constructor(data?: ICloseTradeDto);

    static fromJS(data: any): CloseTradeDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface ICloseTradeDto {
    balance?: number | undefined;
    exitPrice?: number | undefined;
    closed?: Date | undefined;
    result?: ResultDto;
}

export declare class CurrencyDto implements ICurrencyDto {
    id: string;
    isoCode: string | undefined;
    name: string | undefined;

    constructor(data?: ICurrencyDto);

    static fromJS(data: any): CurrencyDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface ICurrencyDto {
    id: string;
    isoCode: string | undefined;
    name: string | undefined;
}

export declare class DecimalNullableUpdatedValue implements IDecimalNullableUpdatedValue {
    value?: number | undefined;

    constructor(data?: IDecimalNullableUpdatedValue);

    static fromJS(data: any): DecimalNullableUpdatedValue;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IDecimalNullableUpdatedValue {
    value?: number | undefined;
}

export declare class ErrorResponse implements IErrorResponse {
    fieldErrors: FieldError[] | undefined;
    commonErrors: string[] | undefined;

    constructor(data?: IErrorResponse);

    static fromJS(data: any): ErrorResponse;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IErrorResponse {
    fieldErrors: FieldError[] | undefined;
    commonErrors: string[] | undefined;
}

export declare class FieldError implements IFieldError {
    identifier: string | undefined;
    messages: string[] | undefined;

    constructor(data?: IFieldError);

    static fromJS(data: any): FieldError;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IFieldError {
    identifier: string | undefined;
    messages: string[] | undefined;
}

export declare class GuidSuccessResponse implements IGuidSuccessResponse {
    data?: string;
    warnings?: string[] | undefined;

    constructor(data?: IGuidSuccessResponse);

    static fromJS(data: any): GuidSuccessResponse;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IGuidSuccessResponse {
    data?: string;
    warnings?: string[] | undefined;
}

export declare class ProblemDetails implements IProblemDetails {
    type?: string | undefined;
    title?: string | undefined;
    status?: number | undefined;
    detail?: string | undefined;
    instance?: string | undefined;

    [key: string]: any;

    constructor(data?: IProblemDetails);

    static fromJS(data: any): ProblemDetails;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IProblemDetails {
    type?: string | undefined;
    title?: string | undefined;
    status?: number | undefined;
    detail?: string | undefined;
    instance?: string | undefined;

    [key: string]: any;
}

export declare class ProfileDto implements IProfileDto {
    id: string;
    name: string | undefined;
    description?: string | undefined;
    isSelected: boolean;

    constructor(data?: IProfileDto);

    static fromJS(data: any): ProfileDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IProfileDto {
    id: string;
    name: string | undefined;
    description?: string | undefined;
    isSelected: boolean;
}

export declare class ReferenceDto implements IReferenceDto {
    id: string;
    type: ReferenceTypeDto;
    link: string | undefined;
    notes?: string | undefined;

    constructor(data?: IReferenceDto);

    static fromJS(data: any): ReferenceDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IReferenceDto {
    id: string;
    type: ReferenceTypeDto;
    link: string | undefined;
    notes?: string | undefined;
}

export declare enum ReferenceTypeDto {
    TradingView = "TradingView",
    Other = "Other"
}

export declare enum ResultDto {
    Win = "Win",
    Mediocre = "Mediocre",
    BreakEven = "BreakEven",
    Loss = "Loss"
}

export declare class ResultDtoNullableUpdatedValue implements IResultDtoNullableUpdatedValue {
    value?: ResultDto;

    constructor(data?: IResultDtoNullableUpdatedValue);

    static fromJS(data: any): ResultDtoNullableUpdatedValue;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IResultDtoNullableUpdatedValue {
    value?: ResultDto;
}

export declare class StringUpdatedValue implements IStringUpdatedValue {
    value?: string | undefined;

    constructor(data?: IStringUpdatedValue);

    static fromJS(data: any): StringUpdatedValue;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IStringUpdatedValue {
    value?: string | undefined;
}

export declare class SuccessResponse implements ISuccessResponse {
    warnings?: string[] | undefined;

    constructor(data?: ISuccessResponse);

    static fromJS(data: any): SuccessResponse;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface ISuccessResponse {
    warnings?: string[] | undefined;
}

export declare class TradeDto implements ITradeDto {
    id?: string;
    assetId?: string;
    asset: string | undefined;
    profileId?: string;
    profile: string | undefined;
    size?: number;
    opened?: Date;
    closed?: Date | undefined;
    balance?: number | undefined;
    result?: ResultDto;
    performance?: number | undefined;
    isClosed?: boolean;
    currencyId?: string;
    currency: string | undefined;
    entry?: number;
    stopLoss?: number | undefined;
    takeProfit?: number | undefined;
    exitPrice?: number | undefined;
    riskRewardRatio?: number | undefined;
    references: ReferenceDto[] | undefined;
    notes?: string | undefined;

    constructor(data?: ITradeDto);

    static fromJS(data: any): TradeDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface ITradeDto {
    id?: string;
    assetId?: string;
    asset: string | undefined;
    profileId?: string;
    profile: string | undefined;
    size?: number;
    opened?: Date;
    closed?: Date | undefined;
    balance?: number | undefined;
    result?: ResultDto;
    performance?: number | undefined;
    isClosed?: boolean;
    currencyId?: string;
    currency: string | undefined;
    entry?: number;
    stopLoss?: number | undefined;
    takeProfit?: number | undefined;
    exitPrice?: number | undefined;
    riskRewardRatio?: number | undefined;
    references: ReferenceDto[] | undefined;
    notes?: string | undefined;
}

export declare class UInt16SuccessResponse implements IUInt16SuccessResponse {
    data?: number;
    warnings?: string[] | undefined;

    constructor(data?: IUInt16SuccessResponse);

    static fromJS(data: any): UInt16SuccessResponse;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IUInt16SuccessResponse {
    data?: number;
    warnings?: string[] | undefined;
}

export declare class UpdateReferenceDto implements IUpdateReferenceDto {
    type?: ReferenceTypeDto;
    link?: string | undefined;
    notes?: StringUpdatedValue;

    constructor(data?: IUpdateReferenceDto);

    static fromJS(data: any): UpdateReferenceDto;

    init(_data?: any): void;

    toJSON(data?: any): any;
}

export interface IUpdateReferenceDto {
    type?: ReferenceTypeDto;
    link?: string | undefined;
    notes?: StringUpdatedValue;
}

export declare class UpdateTradeDto implements IUpdateTradeDto {
    assetId?: string | undefined;
    profileId?: string | undefined;
    opened?: Date | undefined;
    closed?: Date | undefined;
    size?: number | undefined;
    result?: ResultDtoNullableUpdatedValue;
    balance?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: DecimalNullableUpdatedValue;
    takeProfit?: DecimalNullableUpdatedValue;
    exitPrice?: DecimalNullableUpdatedValue;
    notes?: StringUpdatedValue;

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
    result?: ResultDtoNullableUpdatedValue;
    balance?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: DecimalNullableUpdatedValue;
    takeProfit?: DecimalNullableUpdatedValue;
    exitPrice?: DecimalNullableUpdatedValue;
    notes?: StringUpdatedValue;
}

export declare class SwaggerResponse<TResult> {
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
