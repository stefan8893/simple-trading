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
    private http;
    private baseUrl;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined;
    constructor(baseUrl?: string, http?: {
        fetch(url: RequestInfo, init?: RequestInit): Promise<Response>;
    });
    /**
     * @return OK
     */
    getAppInfo(): Promise<SwaggerResponse<ApiInfo>>;
    protected processGetAppInfo(response: Response): Promise<SwaggerResponse<ApiInfo>>;
    /**
     * @return OK
     */
    getTrade(tradeId: string): Promise<SwaggerResponse<TradeDto>>;
    protected processGetTrade(response: Response): Promise<SwaggerResponse<TradeDto>>;
    /**
     * @param body (optional)
     * @return OK
     */
    updateTrade(tradeId: string, body: UpdateTradeDto | undefined): Promise<SwaggerResponse<SuccessResponse>>;
    protected processUpdateTrade(response: Response): Promise<SwaggerResponse<SuccessResponse>>;
    /**
     * @return OK
     */
    deleteTrade(tradeId: string): Promise<SwaggerResponse<SuccessResponse>>;
    protected processDeleteTrade(response: Response): Promise<SwaggerResponse<SuccessResponse>>;
    /**
     * @param body (optional)
     * @return OK
     */
    addTrade(body: AddTradeDto | undefined): Promise<SwaggerResponse<GuidSuccessResponse>>;
    protected processAddTrade(response: Response): Promise<SwaggerResponse<GuidSuccessResponse>>;
    /**
     * @param body (optional)
     * @return OK
     */
    closeTrade(tradeId: string, body: CloseTradeDto | undefined): Promise<SwaggerResponse<SuccessResponse>>;
    protected processCloseTrade(response: Response): Promise<SwaggerResponse<SuccessResponse>>;
    /**
     * @return OK
     */
    getReference(tradeId: string, referenceId: string): Promise<SwaggerResponse<ReferenceDto>>;
    protected processGetReference(response: Response): Promise<SwaggerResponse<ReferenceDto>>;
    /**
     * @param body (optional)
     * @return OK
     */
    updateReference(tradeId: string, referenceId: string, body: UpdateReferenceDto | undefined): Promise<SwaggerResponse<SuccessResponse>>;
    protected processUpdateReference(response: Response): Promise<SwaggerResponse<SuccessResponse>>;
    /**
     * @return OK
     */
    deleteReference(tradeId: string, referenceId: string): Promise<SwaggerResponse<SuccessResponse>>;
    protected processDeleteReference(response: Response): Promise<SwaggerResponse<SuccessResponse>>;
    /**
     * @return OK
     */
    getReferences(tradeId: string): Promise<SwaggerResponse<ReferenceDto[]>>;
    protected processGetReferences(response: Response): Promise<SwaggerResponse<ReferenceDto[]>>;
    /**
     * @param body (optional)
     * @return OK
     */
    addReference(tradeId: string, body: AddReferenceDto | undefined): Promise<SwaggerResponse<GuidSuccessResponse>>;
    protected processAddReference(response: Response): Promise<SwaggerResponse<GuidSuccessResponse>>;
    /**
     * @return OK
     */
    deleteReferences(tradeId: string): Promise<SwaggerResponse<UInt16SuccessResponse>>;
    protected processDeleteReferences(response: Response): Promise<SwaggerResponse<UInt16SuccessResponse>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getAssets(searchTerm: string | undefined): Promise<SwaggerResponse<AssetDto[]>>;
    protected processGetAssets(response: Response): Promise<SwaggerResponse<AssetDto[]>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getProfiles(searchTerm: string | undefined): Promise<SwaggerResponse<ProfileDto[]>>;
    protected processGetProfiles(response: Response): Promise<SwaggerResponse<ProfileDto[]>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getCurrencies(searchTerm: string | undefined): Promise<SwaggerResponse<CurrencyDto[]>>;
    protected processGetCurrencies(response: Response): Promise<SwaggerResponse<CurrencyDto[]>>;
}
export declare class AddReferenceDto implements IAddReferenceDto {
    type?: ReferenceTypeDto;
    link?: string | undefined;
    notes?: string | undefined;
    constructor(data?: IAddReferenceDto);
    init(_data?: any): void;
    static fromJS(data: any): AddReferenceDto;
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
    init(_data?: any): void;
    static fromJS(data: any): AddTradeDto;
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
    init(_data?: any): void;
    static fromJS(data: any): ApiInfo;
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
    init(_data?: any): void;
    static fromJS(data: any): AssetDto;
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
    init(_data?: any): void;
    static fromJS(data: any): CloseTradeDto;
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
    init(_data?: any): void;
    static fromJS(data: any): CurrencyDto;
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
    init(_data?: any): void;
    static fromJS(data: any): DecimalNullableUpdatedValue;
    toJSON(data?: any): any;
}
export interface IDecimalNullableUpdatedValue {
    value?: number | undefined;
}
export declare class ErrorResponse implements IErrorResponse {
    fieldErrors: FieldError[] | undefined;
    commonErrors: string[] | undefined;
    constructor(data?: IErrorResponse);
    init(_data?: any): void;
    static fromJS(data: any): ErrorResponse;
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
    init(_data?: any): void;
    static fromJS(data: any): FieldError;
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
    init(_data?: any): void;
    static fromJS(data: any): GuidSuccessResponse;
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
    init(_data?: any): void;
    static fromJS(data: any): ProblemDetails;
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
    init(_data?: any): void;
    static fromJS(data: any): ProfileDto;
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
    init(_data?: any): void;
    static fromJS(data: any): ReferenceDto;
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
    init(_data?: any): void;
    static fromJS(data: any): ResultDtoNullableUpdatedValue;
    toJSON(data?: any): any;
}
export interface IResultDtoNullableUpdatedValue {
    value?: ResultDto;
}
export declare class StringUpdatedValue implements IStringUpdatedValue {
    value?: string | undefined;
    constructor(data?: IStringUpdatedValue);
    init(_data?: any): void;
    static fromJS(data: any): StringUpdatedValue;
    toJSON(data?: any): any;
}
export interface IStringUpdatedValue {
    value?: string | undefined;
}
export declare class SuccessResponse implements ISuccessResponse {
    warnings?: string[] | undefined;
    constructor(data?: ISuccessResponse);
    init(_data?: any): void;
    static fromJS(data: any): SuccessResponse;
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
    init(_data?: any): void;
    static fromJS(data: any): TradeDto;
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
    init(_data?: any): void;
    static fromJS(data: any): UInt16SuccessResponse;
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
    init(_data?: any): void;
    static fromJS(data: any): UpdateReferenceDto;
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
    init(_data?: any): void;
    static fromJS(data: any): UpdateTradeDto;
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
    constructor(message: string, status: number, response: string, headers: {
        [key: string]: any;
    }, result: any);
    protected isSimpleTradingClientException: boolean;
    static isSimpleTradingClientException(obj: any): obj is SimpleTradingClientException;
}
