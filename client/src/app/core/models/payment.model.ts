export interface PaymentIntentResponse {
  clientSecret: string;
  paymentIntentId: string;
  amount: number;
}

export interface PaymentConfigResponse {
  publishableKey: string;
}
