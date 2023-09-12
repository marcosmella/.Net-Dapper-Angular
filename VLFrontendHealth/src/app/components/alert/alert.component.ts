import { Component, OnInit, OnDestroy, Input, ViewChild, ElementRef } from "@angular/core";
import { Subscription } from "rxjs/internal/Subscription";

import { AlertService, Message } from "../../services/alert.service";

@Component({
	selector: "alert",
	templateUrl: "alert.component.html",
	styleUrls: ["./alert.component.scss"]
})
export class AlertComponent implements OnInit, OnDestroy {
	@Input() id: string;
	public message: Message;
	@ViewChild("alert", { static: false }) alertRef: ElementRef;

	private subscription: Subscription;
	private clear: Subscription;

	constructor(private alertService: AlertService) {}

	ngOnInit(): void {
		this.subscription = this.alertService.getMessage().subscribe((message: Message) => {
			if (!this.id && !message.id) {
				this.setMessage(message);
			}

			if (this.id === message.id) {
				this.alertRef.nativeElement.focus();
				this.setMessage(message);
			}
		});

		this.clear = this.alertService.clear().subscribe((alertsId: Array<string>) => {
			if (!alertsId) {
				this.message = null;
				return;
			}

			alertsId.forEach((element) => {
				if (element === this.id) {
					this.message = null;
				}
			});
		});
	}

	setMessage(message: Message): void {
		if (message.list === null) {
			this.message = null;
			return;
		}
		this.message = message;
	}

	ngOnDestroy(): void {
		this.subscription.unsubscribe();
		this.clear.unsubscribe();
	}
}
