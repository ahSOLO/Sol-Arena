mergeInto(LibraryManager.library, {
	
	newgroundsioPluginExists: function() {
		return true;
	},
	
	newgroundsioPromptIsOpen: function() {
		var prompt = document.getElementById("__ngio_login_prompt");
		return prompt ? true:false;
	},
	
	newgroundsioSaveSessionId: function(id) {
		var session_id = Pointer_stringify(id);
		localStorage.setItem("__ngio_session_id", session_id);
	},
	
	newgroundsioLoadSessionId: function() {
		var session_id = localStorage.getItem("__ngio_session_id");
		if (!session_id) session_id = "";
		var buffer = _malloc(session_id.length+1);
		writeStringToMemory(session_id, buffer);
        return buffer;
	},
	
	newgroundsioClosePassportPrompt: function() {
		var prompt = document.getElementById("__ngio_login_prompt");
		if (prompt) {
			prompt.parentElement.removeChild(prompt);
			return true;
		}
		
		return false;
	},
	
	newgroundsioUserCancelledPrompt: function() {
		var cancelled_state = document.getElementById("__ngio_login_cancelled");
		if (!cancelled_state) {
			console.log("cancelled_state ", "undefined");
			return false;
		}
		console.log("cancelled_state ", cancelled_state.value);
		var cancelled = cancelled_state.value == "1" ? true:false;
		cancelled_state.value = "0";

		return cancelled;
	},
	
	newgroundsioOpenPassport: function(url) {
		var passport_url = Pointer_stringify(url);
		window.open(passport_url, "_newgrounds-passport_");
	},
	
	newgroundsioOpenPassportPrompt: function(url) {
		
		var passport_url = Pointer_stringify(url);
		console.log("LOAD "+passport_url);
		
		var prompt = document.getElementById("__ngio_login_prompt");
		if (prompt) return false;
		
		var cancelled_state = document.getElementById("__ngio_login_cancelled");
		if (!cancelled_state) {
			cancelled_state = document.createElement('input');
			cancelled_state.setAttribute("type","hidden");
			cancelled_state.setAttribute("id","__ngio_login_cancelled");
			document.body.appendChild(cancelled_state);
		}
		
		cancelled_state.value = "0";
		
		function closePrompt() {
			if (prompt) prompt.parentElement.removeChild(prompt);
		}
		
		function openPrompt() {
			closePrompt();
			
			prompt = document.createElement('div');
			prompt.id = "__ngio_login_prompt";

			prompt.style.position = "absolute";
			prompt.style.top = "0px";
			prompt.style.left = "0px";
			prompt.style.width = "100%";
			prompt.style.height = "100%";

			var div_styles = [
				"-webkit-box-shadow: 4px 4px 13px 0px rgba(0,0,0,0.75)",
				"-moz-box-shadow: 4px 4px 13px 0px rgba(0,0,0,0.75)",
				"box-shadow: 4px 4px 13px 0px rgba(0,0,0,0.75)",
				"position: absolute",
				"border: 2px solid black",
				"border-radius: 8px",
				"background-color: #353540",
				"top: 50%",
				"left: 50%",
				"transform: translate(-50%, -50%)",
				"padding: 20px 30px 26px 30px",
				"min-width: 385px",
				"max-width: 674px"
			];
	
			var h2_styles = [
				"line-height: 42px",
				"text-align: right",
				"color: #FFCC00",
				"font-size: 22px",
				"font-family: Arial,Helvetica,sans-serif",
				"margin: 0px 0px 0px 0px",
				"background: url('data:image/png;base64,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') top left no-repeat",
				"background-size: 167px 42px"
			];
	
			var p_styles = [
				"display: block",
				"text-align: center",
				"margin-bottom: 24px",
				"line-height: 22px",
				"color: white",
				"font-size: 15px",
				"font-family: Arial,Helvetica,sans-serif"
			];
	
			var btn_styles = [
				"width: 100px",
				"-webkit-border-radius: 14",
				"-moz-border-radius: 14",
				"border: 2px solid white",
				"border-radius: 14px",
				"font-family: Arial",
				"color: #ffffff",
				"font-size: 16px",
				"padding: 4px 8px 4px 8px",
				"text-decoration: none",
				"cursor: pointer"
			];
	
			var html =		'<div style="'+div_styles.join("; ")+'">';
			html	+=			'<h2 id="__ngio_login_label" style="'+h2_styles.join("; ")+'">';
			html	+=				'Launch Passport?';
			html	+=			'</h2>';
			html	+=			'<p id="__ngio_login_txt" style="'+p_styles.join("; ")+'">';
			html	+=				'Newgrounds Passport will allow you to log in (or sign up) with a Newgrounds.com, Facebook or Google+ Account.';
			html	+=			'</p>';
			html	+=			'<button id="__ngio_cancel_btn" style="float:left; '+btn_styles.join("; ")+'; background-color:#AD1111">'
			html	+=				'No Thanks'
			html	+=			'</button>'
			html	+=			'<button id="__ngio_login_btn" style="float:right; '+btn_styles.join("; ")+'; background-color:#0A690A">';
			html	+=				'Proceed';
			html	+=			'</button>';
			html	+=		'</div>';

			prompt.innerHTML = html;

			document.body.appendChild(prompt);

			var login = document.getElementById("__ngio_login_btn");
			var cancel = document.getElementById("__ngio_cancel_btn");
			
			function openPassport() {
				if (document.location.hostname.indexOf("ungrounded.net") >= 0 || document.location.hostname.indexOf("newgrounds.com") >= 0) {
					parent.postMessage({
						success:true,
						event:'requestLogin'
					},'*');
				} else {
					window.open(passport_url, "_newgrounds-passport_");
				}
			}
			
			cancel.addEventListener("click", function() {
				cancelled_state.value = "1";
				closePrompt();
			});
			
			login.addEventListener("click", function() {
				openPassport();
				closePrompt();
			});
			
		}
		
		openPrompt();
		return true;
	},
	
	newgroundsioOpenUrlInNewTab: function(url) {
		var url = Pointer_stringify(url);
		if (confirm("Open the following URL in a new browser window?\n\n"+url)) {
			window.open(url, "_blank");
		}
	},
	
	newgroundsioAddPluginScripts: function() {
		
		var prefix = "newgroundsio_plugin_url_";
		var popup_id = prefix+"popup";
		var popup_body_id = prefix+"popup_body";
		var popup_a_confirm_id = prefix+"popup_a_confirm";
		var popup_a_cancel_id = prefix+"popup_a_cancel";
		var popup_summary_id = prefix+"popup_summary";
		var popup_button_class = prefix+"popup_btn";
		var popup_block_class = prefix+"block";
		
		var popup_container = document.getElementById(popup_id);
		if (popup_container) return false;
		
		var existing_styles = [];
		var s, r;
		for(s=0; s<document.styleSheets.length; s++) {
			var rules = document.styleSheets[s].rules ? document.styleSheets[s].rules:document.styleSheets[s].cssRules;
			for (r=0; r<rules.length; r++) {
				if (rules[r].selectorText.indexOf(prefix) > -1) existing_styles.push(rules[r].selectorText);
			}
		}
		function styleExists(selector) {
			return existing_styles.indexOf(selector) > -1;
		}
		
		popup_style = document.createElement('style');
		popup_style.type = "text/css";
		
		var style;
		var popup_style_text = "\n";
		
		style = "div."+popup_id;
		
		if (!styleExists(style)) {
			popup_style_text += style+" {\n";
			popup_style_text += "	position: fixed;\n";
			popup_style_text += "	width: 100%;\n";
			popup_style_text += "	height: 100%;\n";
			popup_style_text += "	background-color: rgba(0,0,0,0.5);\n";
			popup_style_text += "	z-index:9999;\n";
			popup_style_text += "	transition: visibility 0.2s, opacity 0.2s linear;\n";
			popup_style_text += "}\n";
			popup_style_text += "\n";
		}
		
		style = "div."+popup_block_class;
		
		if (!styleExists(style)) {
			popup_style_text += style+" {\n";
			popup_style_text += "	text-align:center;\n";
			popup_style_text += "}\n";
			popup_style_text += "\n";
		}
		
		style = "div."+popup_body_id;
		
		if (!styleExists(style)) {
			popup_style_text += style+" {\n";
			popup_style_text += "	border: 2px solid black;\n";
			popup_style_text += "	-webkit-box-shadow: 4px 2px 14px 0px rgba(0,0,0,0.75);\n";
			popup_style_text += "	-moz-box-shadow: 4px 2px 14px 0px rgba(0,0,0,0.75);\n";
			popup_style_text += "	box-shadow: 4px 2px 14px 0px rgba(0,0,0,0.75);\n";
			popup_style_text += "	border-radius: 5px;\n";
			popup_style_text += "	position: absolute;\n";
			popup_style_text += "	padding: 12px;\n";
			popup_style_text += "	top: 50%;\n";
			popup_style_text += "	left: 50%;\n";
			popup_style_text += "	min-width: 200px;\n";
			popup_style_text += "	transform: translate(-50%, -50%);\n";
			popup_style_text += "	background-color: #353540;\n";
			popup_style_text += "}\n";
			popup_style_text += "\n";
		}
		
		style = "p."+popup_summary_id;
		
		if (!styleExists(style)) {
			popup_style_text += style+" {\n";
			popup_style_text += "	color: white;\n";
			popup_style_text += "	margin: 8px;\n";
			popup_style_text += "	text-align: center;\n";
			popup_style_text += "	font-size: 18px;\n";
			popup_style_text += "	font-family: Verdana, Arial, Helvetica, Sans;\n";
			popup_style_text += "}\n";
			popup_style_text += "\n";
			
			popup_style_text += "a."+popup_button_class+":hover {\n";
			popup_style_text += "	border: 2px solid white;\n";
			popup_style_text += "	color: white;\n";
			popup_style_text += "}\n";
			popup_style_text += "\n";
		}
		
		style = "a."+popup_a_confirm_id;
		
		if (!styleExists(style)) {
			popup_style_text += style+" {\n";
			popup_style_text += "	color: white;\n";
			popup_style_text += "	border-radius: 5px;\n";
			popup_style_text += "	border: 2px solid white;\n";
			popup_style_text += "	display: inline-block;\n";
			popup_style_text += "	width: 90px;\n";
			popup_style_text += "	padding: 4px 0px 4px 0px;\n";
			popup_style_text += "	margin: 4px;\n";
			popup_style_text += "	text-align: center;\n";
			popup_style_text += "	font-family: Verdana, Arial, Helvetica, Sans;\n";
			popup_style_text += "	font-size: 14px;\n";
			popup_style_text += "	font-weight: bold;\n";
			popup_style_text += "	text-decoration: none;\n";
			popup_style_text += "	background-color: #339900;\n";
			popup_style_text += "}\n";
			popup_style_text += "\n";

			popup_style_text += style+":hover {\n";
			popup_style_text += "	background-color: #66cc33;\n";
			popup_style_text += "}\n";
			popup_style_text += "\n";
		}
		
		style = "a."+popup_a_cancel_id;
		
		if (!styleExists(style)) {
			popup_style_text += style+" {\n";
			popup_style_text += "	font-family: Verdana, Arial, Helvetica, Sans;\n";
			popup_style_text += "	font-size: 9px;\n";
			popup_style_text += "	display: inline-block;\n";
			popup_style_text += "	color: #C4C4E6;\n";
			popup_style_text += "	margin: 12px;\n";
			popup_style_text += "}\n";
		}
		
		popup_style.innerHTML = popup_style_text;
		
		popup_container = document.createElement('div');
		popup_container.id = popup_id;
		popup_container.className = popup_id;
		popup_container.style.visibility = "hidden";
		popup_container.style.opacity = "0";
		
		var popup_body = document.createElement('div');
		popup_body.id = popup_body_id;
		popup_body.className = popup_body_id;
		
		var popup_summary = document.createElement('p');
		popup_summary.id = popup_summary_id;
		popup_summary.className = popup_summary_id;
		popup_summary.innerHTML = "Open URL in new tab?";
		
		var popup_a_confirm = document.createElement('a');
		popup_a_confirm.target = "";
		popup_a_confirm.text = "OK";
		popup_a_confirm.id = popup_a_confirm_id;
		popup_a_confirm.className = popup_a_confirm_id;
		popup_a_confirm.onclick = function(e) {
			popup_container.style.visibility = 'hidden';
			popup_container.style.opacity = "0";
		}
		
		var popup_a_cancel = document.createElement('a');
		popup_a_cancel.target = "";
		popup_a_cancel.text = "CANCEL";
		popup_a_cancel.href = "#";
		popup_a_cancel.id = popup_a_cancel_id;
		popup_a_cancel.className = popup_a_cancel_id;
		popup_a_cancel.onclick = function(e) {
			popup_container.style.visibility = 'hidden';
			popup_container.style.opacity = "0";
			e.preventDefault();
		}
		
		document.body.appendChild(popup_style);
		
		var block1 = document.createElement('div');
		block1.className = popup_block_class;
		
		var block2 = document.createElement('div');
		block2.className = popup_block_class;
		
		popup_body.appendChild(popup_summary);
		block1.appendChild(popup_a_confirm);
		block2.appendChild(popup_a_cancel);
		popup_body.appendChild(block1);
		popup_body.appendChild(block2);
		popup_container.appendChild(popup_body);
		document.body.appendChild(popup_container);
		
		var script = document.createElement("script");
		script.type = "text/javascript";
		script.text = "";
		
		script.text += "newgroundsio_plugin_url_popup = document.getElementById('"+popup_id+"');";
		script.text += "newgroundsio_plugin_url_popup_a_confirm = document.getElementById('"+popup_a_confirm_id+"');";
		script.text += "function newgroundsio_plugin_openURL(url,target) {\n";
		script.text += "	newgroundsio_plugin_url_popup_a_confirm.href = url\n";
		script.text += "	newgroundsio_plugin_url_popup_a_confirm.target = target\n";
		script.text += "	newgroundsio_plugin_url_popup.style.visibility = 'visible'\n";
		script.text += "	newgroundsio_plugin_url_popup.style.opacity = '1'\n";
		script.text += "}";
		
		document.body.appendChild(script);
		
		return true;
	}
});