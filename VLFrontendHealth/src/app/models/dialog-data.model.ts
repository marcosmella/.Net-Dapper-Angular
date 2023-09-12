export interface DialogData {
	title: string;
	message: string;
	noButtonMessage: string;
	okButtonMessage: string;
	warningToast?: string;
	attentionToast?: string;
	action?: () => void;
}
